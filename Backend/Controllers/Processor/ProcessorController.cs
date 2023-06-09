using AltV.Net;
using Backend.Controllers.Processor.Interface;
using Backend.Modules.Inventory;
using Backend.Services.Processor.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;
using Backend.Utils.Models;

namespace Backend.Controllers.Processor
{
	public class ProcessorController : IProcessorController
	{
		private readonly IProcessorService _processorService;

		public ProcessorController(IProcessorService processorService)
		{
			_processorService = processorService;

			_processorService.Processors.ForEach(LoadProcessor);
		}

		public void LoadProcessor(ProcessorModel model)
		{
			ClShape shape = (ClShape)Alt.CreateColShapeSphere(model.Position, 2f);
			shape.Id = model.Id;
			shape.ShapeType = ColshapeType.PROCESSOR;
			shape.Size = 2f;

			PedStreamer.AddObject(new Ped(0xE7565327, model.Ped.X, model.Ped.Y, model.Ped.Z, model.Ped.H, true, true, 0));
		}

		public Task ProcessVehicle(ClPlayer player, int id, int vehId)
		{
			var veh = ClVehicle.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == vehId);
			if (veh == null || veh.DbModel == null) return Task.CompletedTask;

			var model = _processorService.GetProcessor(id);
			if (model == null || veh.Position.Distance(model.Position) > 20) return Task.CompletedTask;

			var neededItem = InventoryModule.GetItemBase(model.NeededItem);
			var getItem = InventoryModule.GetItemBase(model.GetItem);
			if (neededItem == null || getItem == null) return Task.CompletedTask;

			var steps = (int)Math.Floor((decimal)(veh.DbModel.Trunk.GetItemsCount(neededItem) / model.NeededItemAmount));

			if (steps < 1)
			{
				player.Notify("VERARBEITER", $"Es müssen mindestens {model.NeededItemAmount}x {neededItem.Name} im Kofferraum liegen!", NotificationType.ERROR);
				return Task.CompletedTask;
			}

			var con1 = veh.DbModel.Trunk.GetInventoryWeight() + getItem.Weight * model.GetItemAmount * steps - neededItem.Weight * model.NeededItemAmount * steps > veh.DbModel.Trunk.MaxWeight;
			var con2 = (Math.Ceiling((decimal)(model.GetItemAmount * steps / getItem.MaxAmount)) - Math.Floor((decimal)model.NeededItemAmount * steps / neededItem.MaxAmount)) > veh.DbModel.Trunk.GetFreeSlots();

			if (con1 || con2)
			{
				player.Notify("VERARBEITER", $"Es ist nicht genug Platz im Fahrzeug!", NotificationType.ERROR);
				return Task.CompletedTask;
			}

			player.StartInteraction(async () =>
			{
				if (veh.Position.Distance(model.Position) > 20f)
				{
					await player.Notify("VERARBEITER", "Das Fahrzeug hat sich zu weit vom Verarbeiter entfernt!", NotificationType.ERROR);
					return;
				}

				if (!veh.DbModel.Trunk.RemoveItem(neededItem, model.NeededItemAmount * steps)) return;
				veh.DbModel.Trunk.AddItem(getItem, model.GetItemAmount * steps);
				await player.Notify("Verarbeiter", "Verarbeitung abgeschlossen!", NotificationType.SUCCESS);
			}, model.Duration * steps);

			return Task.CompletedTask;
		}

		public Task ProcessInventory(ClPlayer player, int id)
		{
			var model = _processorService.GetProcessor(id);
			if (model == null || player.Position.Distance(model.Position) > 20) return Task.CompletedTask;

			var neededItem = InventoryModule.GetItemBase(model.NeededItem);
			var getItem = InventoryModule.GetItemBase(model.GetItem);
			if(neededItem == null || getItem == null) return Task.CompletedTask;

			var steps = (int)Math.Floor((decimal)(player.DbModel.Inventory.GetItemsCount(neededItem) / model.NeededItemAmount));

			if (steps < 1)
			{
				player.Notify("VERARBEITER", $"Du benötigst mindestens {model.NeededItemAmount}x {neededItem.Name}!", NotificationType.ERROR);
				return Task.CompletedTask;
			}

			var con1 = player.DbModel.Inventory.GetInventoryWeight() + getItem.Weight * model.GetItemAmount * steps - neededItem.Weight * model.NeededItemAmount * steps > player.DbModel.Inventory.MaxWeight;
			var con2 = (Math.Ceiling((decimal)(model.GetItemAmount * steps / getItem.MaxAmount)) - Math.Floor((decimal)model.NeededItemAmount * steps / neededItem.MaxAmount)) > player.DbModel.Inventory.GetFreeSlots();

			if (con1 || con2)
			{
				player.Notify("VERARBEITER", $"Du hast nicht genug Platz!", NotificationType.ERROR);
				return Task.CompletedTask;
			}

			player.StartInteraction(async () =>
			{
				if(player.Position.Distance(model.Position) > 20f)
				{
					await player.Notify("VERARBEITER", "Du hast dich zu weit vom Verarbeiter entfernt!", NotificationType.ERROR);
					return;
				}
				
				if (!player.DbModel.Inventory.RemoveItem(neededItem, model.NeededItemAmount * steps)) return;
				player.DbModel.Inventory.AddItem(getItem, model.GetItemAmount * steps);
				await player.Notify("Verarbeiter", "Verarbeitung abgeschlossen!", NotificationType.SUCCESS);
			}, model.Duration * steps);

			return Task.CompletedTask;
		}

		public async Task AddProcessor(ProcessorModel model)
		{
			await _processorService.AddProcessor(model);
			LoadProcessor(model);
		}
	}
}