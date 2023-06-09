using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Player.Interface;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.Player
{
	public class PlayerModule : Module<PlayerModule>, IEventPlayerDamage, IEventColshape, IEventPlayerDisconnect, IEventIntervalMinute
	{
		private readonly IPlayerController _playerController;
		private readonly IVehicleService _vehicleService;

		public PlayerModule(IEventController eventController, IPlayerController playerController, IVehicleService vehicleService) : base("Player")
		{
			_playerController = playerController;
			_vehicleService = vehicleService;

			eventController.OnClient("Server:Player:Kick", Kick);
			eventController.OnClient<int, string>("Server:Player:GiveMoney", GiveMoney);
			eventController.OnClient("Server:Player:StopInteraction", StopInteraction);
			eventController.OnClient<string>("Server:Player:UpdateAmmo", UpdateAmmo);
		}

		private void Kick(ClPlayer player, string eventKey)
		{
			player.Kick("Du wurdest gekicked!");
		}

		private async void GiveMoney(ClPlayer player, string eventKey, int targetId, string amountText)
		{
			if (targetId < 0 || !int.TryParse(amountText, out var amount) || amount < 1) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Id == targetId);
			if (target == null) return;

			if(player.DbModel.Money < amount)
			{
				await player.Notify("Information", "Du hast nicht so viel Geld dabei!", NotificationType.ERROR);
				return;
			}

			await player.RemoveMoney(amount);
			await target.AddMoney(amount);
			await player.Notify("Information", $"Du hast jemandem ${amount} gegeben.", NotificationType.INFO);
			await target.Notify("Information", $"Du hast von jemandem ${amount} erhalten.", NotificationType.INFO);
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player) return Task.CompletedTask;

			var player = (ClPlayer)entity;
			player.CurrentShape = entered ? shape : null;
			player.Emit(entered ? "Client:PlayerModule:EnterColshape" : "Client:PlayerModule:ExitColshape", shape.Id, (int)shape.ShapeType);

			return Task.CompletedTask;
		}

		public async Task OnEveryMinute()
		{
			foreach(var player in ClPlayer.All.Where(x => x.DbModel != null))
			{
				player.DbModel.PaycheckTicks++;
				player.DbModel.Starvation = Math.Clamp(player.DbModel.Starvation-1, 0, 100);
				player.DbModel.Hydration = Math.Clamp(player.DbModel.Hydration-1, 0, 100);
				player.DbModel.Strength = (float)Math.Clamp(player.DbModel.Strength - 0.5, 80, 120);

				if (player.DbModel.Starvation % 25 == 0 || player.DbModel.Hydration % 25 == 0 || player.DbModel.Strength % 20 == 0)
					await player.SetFood(player.DbModel.Starvation, player.DbModel.Hydration, player.DbModel.Strength, false);

				if (player.DbModel.Starvation == 0 || player.DbModel.Hydration == 0)
					await player.SetHealth((ushort)(player.Health - 5), player.Armor, true);

				if (player.DbModel.PaycheckTicks < 60) continue;

				player.DbModel.PaycheckTicks = 0;

				player.DbModel.SocialCredit += player.DbModel.Level * 75;

				var paycheck = 0;
				if (player.DbModel.Duty)
				{
					switch (player.DbModel.Team)
					{
						case 1:
							paycheck = 4000 + player.DbModel.TeamRank * 250;
							break;
						case 2:
							paycheck = 4000 + player.DbModel.TeamRank * 450;
							break;
						case 3:
							paycheck = 4000 + player.DbModel.TeamRank * 750;
							break;
					}
				}

				// Todo: add to bank history
				if (paycheck > 0)
				{
					player.DbModel.BankMoney += paycheck;
					await player.Notify("BANKING", $"Du hast einen Paycheck erhalten!", NotificationType.INFO);
				}

				var tax = _vehicleService.Vehicles.Where(x => x.Type == VehicleType.PLAYER && x.Owner == player.DbModel.Id).Sum(x => _vehicleService.GetVehicleInfo(x.InfoModelId).Result?.Tax);
				if (player.DbModel.BankMoney < tax)
				{
					await player.Notify("BANKING", "Da hast nicht genug Geld um deine Steuern zu bezahlen!", NotificationType.INFO);
					return;
				}

				player.DbModel.Xp++;

				if (player.DbModel.Xp < player.DbModel.Level * 4) continue;

				player.DbModel.Xp = 0;
				player.DbModel.Level++;
				await player.Notify("LEVEL UP", "Glückwunsch, dein Level hat sich soeben erhöht!", NotificationType.SUCCESS);
			}
		}

		public Task OnPlayerDamage(ClPlayer player, IEntity attacker, ushort oldHealth, ushort oldArmor, ushort oldMaxHealth, ushort oldMaxArmor, uint weapon, ushort healthDamage, ushort armorDamage)
		{
			player.SetHealth(player.Health, player.Armor, false);

			return Task.CompletedTask;
		}

		public async Task OnPlayerDisconnect(ClPlayer player, string reason)
		{
			await _playerController.SavePlayer(player);
		}

		private async void StopInteraction(ClPlayer player, string eventKey)
		{
			if (!player.Interaction) return;

			await player.StopInteraction();
		}

		private void UpdateAmmo(ClPlayer player, string eventKey, string json)
		{
			if (player.DbModel == null || player.IsInGangwar) return;

			var data = JsonConvert.DeserializeObject<List<ClientWeaponModel>>(json);
			if (data == null) return;

			foreach(var weapon in data)
			{
				var weaponModel = player.DbModel.Loadout.FirstOrDefault(x => x.Hash == weapon.hash);
				if (weaponModel == null) continue;

				weaponModel.Ammo = weapon.ammo;
			}
		}
	}
}