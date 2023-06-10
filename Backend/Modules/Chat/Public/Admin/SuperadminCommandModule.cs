using Backend.Utils.Models;
using Backend.Utils.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Models.Entities;
using Backend.Services.Account.Interface;
using Backend.Modules.Inventory;
using AltV.Net;
using Backend.Utils.Models.ClothesShop;

namespace Backend.Modules.Chat.Public.Admin
{
	public class SuperadminCommandModule : CommandModule<SuperadminCommandModule>
	{
		private readonly IAccountService _accountService;

		public SuperadminCommandModule(IEventController eventController, IAccountService accountService) : base("SuperadminCommand", AdminRank.SUPERADMINISTRATOR)
		{
			_accountService = accountService;

			eventController.OnClient<int, int, int, string>("Server:Command:setprop", SetProp);
			eventController.OnClient<int, int, int, string>("Server:Command:setclothes", SetClothes);
			eventController.OnClient<string, int>("Server:Command:setmoney", SetMoney);
			eventController.OnClient<uint>("Server:Command:rmweaponcomp", RemoveWeaponComponent);
			eventController.OnClient<uint>("Server:Command:addweaponcomp", AddWeaponComponent);
			eventController.OnClient<byte>("Server:Command:setweaponcolor", SetWeaponColor);
			eventController.OnClient<int, int>("Server:Command:giveitem", GiveItem);
			eventController.OnClient<string, int>("Server:Command:setadmin", SetAdmin);
		}

		private void SetProp(ClPlayer player, string eventKey, int slot, int drawable, int texture, string dlc = "")
		{
			if (!CheckPermission(player)) return;

			var hash = Alt.Hash(dlc);

			if (drawable == -1)
			{
				player.ClearProps((byte)slot);
			}
			else
			{
				if (dlc == "") player.SetProps((byte)slot, (ushort)drawable, (byte)texture);
				else player.SetDlcProps((byte)slot, (ushort)drawable, (byte)texture, hash);
			}

			var model = new ClothingModel(slot, drawable, texture, hash);

			switch (slot)
			{
				case 0:
					player.DbModel.Clothes.Hat = model;
					break;
				case 1:
					player.DbModel.Clothes.Glasses = model;
					break;
				case 2:
					player.DbModel.Clothes.Ears = model;
					break;
				case 6:
					player.DbModel.Clothes.Watch = model;
					break;
				case 7:
					player.DbModel.Clothes.Bracelet = model;
					break;
			}
		}

		private void SetClothes(ClPlayer player, string eventKey, int slot, int drawable, int texture, string dlc = "")
		{
			if (!CheckPermission(player)) return;

			var hash = Alt.Hash(dlc);

			if (dlc == "")
			{
				player.SetClothes((byte)slot, (ushort)drawable, (byte)texture, 0);
			}
			else
			{
				player.SetDlcClothes((byte)slot, (ushort)drawable, (byte)texture, 0, hash);
			}

			var model = new ClothingModel(slot, drawable, texture, hash);

			switch (slot)
			{
				case 1:
					player.DbModel.Clothes.Mask = model;
					break;
				case 11:
					player.DbModel.Clothes.Top = model;
					break;
				case 8:
					player.DbModel.Clothes.Undershirt = model;
					break;
				case 3:
					player.DbModel.Clothes.Body = model;
					break;
				case 4:
					player.DbModel.Clothes.Leg = model;
					break;
				case 6:
					player.DbModel.Clothes.Shoe = model;
					break;
				case 9:
					player.DbModel.Clothes.Armor = model;
					break;
				case 5:
					player.DbModel.Clothes.Bag = model;
					break;
				case 7:
					player.DbModel.Clothes.Accessories = model;
					break;
			}
		}

		private async void SetMoney(ClPlayer player, string eventKey, string targetName, int money)
		{
			if (!CheckPermission(player)) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Name == targetName);
			if(target == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden!", NotificationType.ERROR);
				return;
			}

			await target.SetMoney(money);
			await target.Notify("Administration", $"Dein Geld wurde von {player.Name} auf ${money} gesetzt!", NotificationType.INFO);
			await player.Notify("Administration", $"Du hast das Geld von {target.Name} auf ${money} gesetzt!", NotificationType.SUCCESS);
		}

		private void RemoveWeaponComponent(ClPlayer player, string eventKey, uint component)
		{
			if (!CheckPermission(player)) return;

			var gun = player.DbModel.Loadout.FirstOrDefault(x => x.Hash == player.CurrentWeapon)!;
			gun.Attatchments.Remove(component);
			player.RemoveWeaponComponent(gun.Hash, component);
		}

		private void AddWeaponComponent(ClPlayer player, string eventKey, uint component)
		{
			if (!CheckPermission(player)) return;

			var gun = player.DbModel.Loadout.FirstOrDefault(x => x.Hash == player.CurrentWeapon)!;
			gun.Attatchments.Add(component);
			player.AddWeaponComponent(gun.Hash, component);
		}

		private void SetWeaponColor(ClPlayer player, string eventKey, byte tintIndex)
		{
			if (!CheckPermission(player)) return;

			var gun = player.DbModel.Loadout.FirstOrDefault(x => x.Hash == player.CurrentWeapon)!;
			gun.TintIndex = tintIndex;
			player.SetWeaponTintIndex(gun.Hash, tintIndex);
		}

		private void GiveItem(ClPlayer player, string eventKey, int itemId, int amount)
		{
			if (!CheckPermission(player) || amount < 1) return;

			var itemModel = InventoryModule.GetItemBase(itemId);
			if (itemModel == null)
			{
				player.Notify("Administration", $"Item {itemId} wurde nicht gefunden.", NotificationType.ERROR);
				return;
			}

			if (!player.DbModel.Inventory.AddItem(itemModel, amount))
				player.Notify("Administration", "Du hast nicht genug Platz.", NotificationType.ERROR);
		}

		private async void SetAdmin(ClPlayer player, string eventKey, string targetName, int rank)
		{
			if (!CheckPermission(player) || rank >= (int)player.DbModel.AdminRank) return;

			var acc = _accountService.GetAccount(targetName);
			if (acc == null)
			{
				await player.Notify("Administration", "Der Spieler konnte nicht gefunden werden.", NotificationType.ERROR);
				return;
			}

			acc.AdminRank = (AdminRank)rank;
			await player.Notify("Administration", $"Du hast den Adminrang von {acc.Name} auf {Enum.GetName(typeof(AdminRank), rank)}", NotificationType.SUCCESS);
		}
	}
}