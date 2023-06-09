using Backend.Controllers.Event.Interface;
using Backend.Modules.Inventory;
using Backend.Services.Workstation.Interface;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Workstation;
using Newtonsoft.Json;

namespace Backend.Modules.Workstation
{
	public class WorkstationModule : Module<WorkstationModule>, IEventIntervalMinute
	{
		private readonly IWorkstationService _workstationService;

		public WorkstationModule(IEventController eventController, IWorkstationService workstationService) : base("Workstation")
		{
			_workstationService = workstationService;

			eventController.OnClient<int, int>("Server:Workstation:Take", Take);
			eventController.OnClient<int, int, string>("Server:Workstation:Start", Start);
			eventController.OnClient<int>("Server:Workstation:Open", Open);
		}

		private async void Take(ClPlayer player, string eventKey, int id, int itemIndex)
		{
			var model = await _workstationService.GetWorkstation(id);
			if (model == null || player.Position.Distance(model.Position) > 2)
			{
				await player.Notify("Workstation", "Es ist ein Fehler aufgetreten!", Utils.Enums.NotificationType.ERROR);
				await player.ShowComponent("Workstation", false);
				return;
			}

			var item = player.DbModel.WorkstationItems.Where(x => x.WorkstationId == id).ToList()[itemIndex];
			if (item == null || item.Ticks < item.Duration) return;

			var itemModel = InventoryModule.GetItemBase(item.OutputItemId)!;

			if(player.DbModel.Inventory.Slots < Math.Ceiling((decimal)item.OutputItemAmount / itemModel.MaxAmount) || player.DbModel.Inventory.GetInventoryWeight() + itemModel.Weight * item.OutputItemAmount > player.DbModel.Inventory.MaxWeight)
			{
				await player.Notify("Workstation", "Du hast nicht genug Platz!", Utils.Enums.NotificationType.ERROR);
				await player.ShowComponent("Workstation", false);
				return;
			}

			player.DbModel.WorkstationItems.Remove(item);
			player.DbModel.Inventory.AddItem(itemModel, item.OutputItemAmount);
			await player.Notify("Workstation", "Du hast einen Auftrag abgeholt.", Utils.Enums.NotificationType.INFO);
		}

		private async void Start(ClPlayer player, string eventKey, int id, int itemIndex, string date)
		{
			var model = await _workstationService.GetWorkstation(id);
			if (model == null || player.Position.Distance(model.Position) > 2)
			{
				await player.Notify("Workstation", "Es ist ein Fehler aufgetreten!", Utils.Enums.NotificationType.ERROR);
				await player.ShowComponent("Workstation", false);
				return;
			}

			var item = model.Items[itemIndex];
			if (item == null) return;

			var itemModel = InventoryModule.GetItemBase(item.InputItem)!;

			if (player.DbModel.WorkstationItems.Where(x => x.WorkstationId == id && x.OutputItemId == item.OutputItem).Count() >= item.Max)
			{
				await player.Notify("Workstation", "Es ist ein Fehler aufgetreten!", Utils.Enums.NotificationType.ERROR);
				await player.ShowComponent("Workstation", false);
				return;
			}

			if(player.DbModel.Money < item.Price)
			{
				await player.Notify("Workstation", "Du hast nicht genug Geld dabei!", Utils.Enums.NotificationType.ERROR);
				await player.ShowComponent("Workstation", false);
				return;
			}

			if(!player.DbModel.Inventory.HasItems(itemModel, item.InputAmount))
			{
				await player.Notify("Workstation", "Du hast nicht genug Material dabei!", Utils.Enums.NotificationType.ERROR);
				await player.ShowComponent("Workstation", false);
				return;
			}

			player.DbModel.Inventory.RemoveItem(itemModel, item.InputAmount);
			await player.RemoveMoney(item.Price);

			player.DbModel.WorkstationItems.Add(new Utils.Models.Player.PlayerWorkstationItemModel(id, item.OutputItem, item.OutputAmount, date, 0, item.Duration));
			await player.Notify("Workstation", "Der Auftrag wurde hinzugefügt!", Utils.Enums.NotificationType.SUCCESS);
		}

		private async void Open(ClPlayer player, string eventKey, int id)
		{
			var model = await _workstationService.GetWorkstation(id);
			if (model == null || player.Position.Distance(model.Position) > 2) return;

			var items = new List<ClientWorkstationItem>();
			model.Items.ForEach(item => items.Add(new ClientWorkstationItem(item)));

			var running = new List<ClientWorkstationRunningItem>();
			foreach (var item in player.DbModel.WorkstationItems.Where(x => x.WorkstationId == id))
				running.Add(new ClientWorkstationRunningItem(item));

			var data = new
			{
				Id = id,
				Items = items,
				Running = running
			};

			await player.ShowComponent("Workstation", true, JsonConvert.SerializeObject(data));
		}

		public Task OnEveryMinute()
		{
			foreach(var player in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.WorkstationItems.Count > 0))
			{
				for(var i = 0; i < _workstationService.Workstations.Count; i++)
				{
					var item = player.DbModel.WorkstationItems.FirstOrDefault(x => x.Ticks < x.Duration && x.WorkstationId == _workstationService.Workstations[i].Id);
					if(item == null) continue;

					item.Ticks++;
				}
			}

			return Task.CompletedTask;
		}
	}
}