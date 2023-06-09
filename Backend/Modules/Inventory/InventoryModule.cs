using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;
using Newtonsoft.Json;
using Backend.Utils.Models.Inventory.Client;
using Backend.Utils.Enums;
using Backend.Utils.Models.Inventory.Database;
using AltV.Net.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Services.Storage.Interface;
using Backend.Services.Team.Interface;
using Backend.Services.House.Interface;

namespace Backend.Modules.Inventory
{
	public class InventoryModule : Module<InventoryModule>
	{
		public static readonly List<ItemBase> ItemModels = new();
        private readonly IStorageService _storageService;
        private readonly ITeamService _teamService;
        private readonly IHouseService _houseService;

        public static InventoryModel Convert(DatabaseInventoryModel inv)
		{
			var inventory = new InventoryModel(new List<ItemModel>(), inv.MaxWeight, inv.Slots, inv.Type);

			for (var i = 0; i < inv.Items.Count; i++)
			{
				var itemBase = GetItemBase(inv.Items[i].ItemId);
				if (itemBase == null) continue;

				inventory.Items.Add(new ItemModel(itemBase, inv.Items[i].Amount, inv.Items[i].Slot));
			}

			return inventory;
		}

		public static List<InventoryModel> Convert(List<DatabaseInventoryModel> inv)
        {
            var list = new List<InventoryModel>();

            inv.ForEach(item =>
            {
                var inventory = new InventoryModel(new List<ItemModel>(), item.MaxWeight, item.Slots, item.Type);

                for (var i = 0; i < item.Items.Count; i++)
                {
                    var itemBase = GetItemBase(item.Items[i].ItemId);
                    if (itemBase == null) continue;

                    inventory.Items.Add(new ItemModel(itemBase, item.Items[i].Amount, item.Items[i].Slot));
                }

				list.Add(inventory);
            });

            return list;
        }

        public static DatabaseInventoryModel Convert(InventoryModel inv)
        {
            var inventory = new DatabaseInventoryModel(new List<DatabaseItemModel>(), inv.MaxWeight, inv.Slots, inv.Type);

            for (var i = 0; i < inv.Items.Count; i++)
            {
                inventory.Items.Add(new DatabaseItemModel(inv.Items[i].Model.Id, inv.Items[i].Amount, inv.Items[i].Slot));
            }

            return inventory;
        }

        public static List<DatabaseInventoryModel> Convert(List<InventoryModel> inv)
        {
			var list = new List<DatabaseInventoryModel>();

            inv.ForEach(item =>
            {
                var inventory = new DatabaseInventoryModel(new List<DatabaseItemModel>(), item.MaxWeight, item.Slots, item.Type);

                for (var i = 0; i < item.Items.Count; i++)
                {
                    inventory.Items.Add(new DatabaseItemModel(item.Items[i].Model.Id, item.Items[i].Amount,
                        item.Items[i].Slot));
                }

				list.Add(inventory);
            });


            return list;
        }

        public static ItemBase? GetItemBase(int? id)
		{
			return ItemModels.FirstOrDefault(x => x.Id == id);
		}

		public InventoryModule(IStorageService storageService, IEventController eventController, ITeamService teamService, IHouseService houseService) : base("Inventory")
        {
            _storageService = storageService;
            _teamService = teamService;
            _houseService = houseService;

            eventController.OnClient<int>("Server:Inventory:Open", Open);
			eventController.OnClient<int, int>("Server:Inventory:Use", UseItem);
			eventController.OnClient<int, int, int>("Server:Inventory:Give", GiveItem);
			eventController.OnClient<int, int>("Server:Inventory:Throw", ThrowItem);
			eventController.OnClient<int, int>("Server:Inventory:MoveAll", MoveAll);
			eventController.OnClient<int, int, int>("Server:Inventory:MoveAmount", MoveAmount);
			eventController.OnClient<int, int, int, int>("Server:Container:MoveAll", ContainerMoveAll);
			eventController.OnClient<int, int, int, int, int>("Server:Container:MoveAmount", ContainerMoveAmount);
			eventController.OnClient<int, int, int, int, bool>("Server:Inventory:MoveAcrossContainer", MoveAcrossContainer);
			eventController.OnClient<int, int, int, int, int, bool>("Server:Inventory:MoveAmountAcrossContainer", MoveAmountAcrossContainer);
			eventController.OnClient<int>("Server:Inventory:PackItem", PackItem);
			eventController.OnClient<int, int>("Server:Inventory:DropWeapon", DropWeapon);
		}

		private void DropWeapon(ClPlayer player, string eventKey, int targetId, int hash)
		{
			if (player.DbModel == null) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.DbModel.Id == targetId);
			if (target == null || target.DbModel == null || (target.DbModel.Alive && !target.DbModel.Roped && !target.DbModel.Cuffed)) return;

			target.RemoveWeapon2((uint)hash);
			player.Notify("Information", "Du hast der Person eine Waffe abgenommen!", NotificationType.SUCCESS);
			target.Notify("Information", "Jemand hat dir eine Waffe abgenommen!", NotificationType.INFO);
		}

		private void PackItem(ClPlayer player, string eventKey, int id)
		{
			switch(id)
			{
				case 1:
					if (!player.DbModel.Phone || !player.DbModel.Inventory.CanCarryItem(ItemModels.FirstOrDefault(x => x.Id == 4), 1)) return;

					player.DbModel.Phone = false;
					player.DbModel.Inventory.AddItem(ItemModels.FirstOrDefault(x => x.Id == 4), 1);
					player.ShowComponent("Inventory", false);
					
					if (!player.DbModel.Laptop  || !player.DbModel.Inventory.CanCarryItem(ItemModels.FirstOrDefault(x => x.Id == 12), 1)) return;
					player.DbModel.Laptop = false;
					player.DbModel.Inventory.AddItem(ItemModels.FirstOrDefault(x => x.Id == 12), 1);

					break;
				case 3:
					if (!player.DbModel.Backpack || player.DbModel.Inventory.Items.Count > 8 || player.DbModel.Inventory.GetInventoryWeight() > 45f) return;

					player.ShowComponent("Inventory", false);
					player.DbModel.Backpack = false;
					player.DbModel.Inventory.Slots = 8;
					player.DbModel.Inventory.MaxWeight = 45f;
					for (var i = 0; i < player.DbModel.Inventory.Items.Count; i++) player.DbModel.Inventory.Items[i].Slot = i+1;
					player.DbModel.Inventory.AddItem(ItemModels.FirstOrDefault(x => x.Id == 5), 1);
					break;
				case 4:
					if (player.CurrentWeapon == (uint)WeaponModel.Fist) return;
					var item = (WeaponItemBase)ItemModels.FirstOrDefault(x => x.Type == ItemType.WEAPON && ((WeaponItemBase)x).Hash == player.CurrentWeapon);
					var ammoModel = ItemModels.FirstOrDefault(x => x.Type == ItemType.AMMO && ((AmmoItemBase)x).WeaponType == item.WeaponType);
					var ammoAmount = (int)Math.Floor((decimal)player.DbModel.Loadout.FirstOrDefault(x => x.Hash == player.CurrentWeapon)!.Ammo / 30);
					if (item == null || (player.DbModel.Inventory.MaxWeight - player.DbModel.Inventory.GetInventoryWeight()) < (item.Weight + ammoModel?.Weight * ammoAmount) || player.DbModel.Inventory.GetFreeSlots() < Math.Ceiling((decimal)(ammoModel == null ? 0 : ammoAmount / ammoModel.MaxAmount)) + 1) return;

					player.PlayAnimation(AnimationType.PACK_GUN);
					player.ShowComponent("Inventory", false);
					player.StartInteraction(() =>
					{
						player.RemoveWeapon2(player.CurrentWeapon);
						player.DbModel.Inventory.AddItem(item, 1);
						player.DbModel.Inventory.AddItem(ammoModel, ammoAmount);
					}, 4000);
					break;
				case 5:
					if (player.Armor < 1) return;

					player.PlayAnimation(AnimationType.PACK_VEST);
					player.ShowComponent("Inventory", false);
					player.StartInteraction(() =>
					{
						var item = ItemModels.FirstOrDefault(x => x.Id == (player.Armor > 145 ? 3 : player.Armor > 95 ? 2 : -1));
						if(item != null) player.DbModel.Inventory.AddItem(item, 1);

						player.SetHealth(player.Health, 0);
						player.DbModel.Clothes.Armor.Drawable = 0;
						player.DbModel.Clothes.Armor.Texture = 0;
						player.DbModel.Clothes.Armor.Dlc = 0;
						player.SetClothes(9, 0, 0, 0);
					}, 4000);
					break;
			}
		}

		private void Open(ClPlayer player, string eventKey, int giveItemTargetId = -1)
		{
			var targetPlayer = ClPlayer.All.FirstOrDefault(x => x.DbModel != null && x.Id == giveItemTargetId);

			int targetId = -1;
			InventoryModel? targetInventory = null;

			if(!player.IsInVehicle || ((ClVehicle)player.Vehicle).DbModel == null)
			{
				var vehicle = ClVehicle.All.FirstOrDefault(x => x.DbModel != null && !x.Locked && !x.TrunkLocked && x.Position.Distance(player.Position) < 5 && x.EngineHealth > -3999);
				if (vehicle != null)
				{
					targetId = vehicle.DbModel!.Id;
					targetInventory = vehicle.DbModel!.Trunk;
				}
				else
				{
					var shape = player.CurrentShape;
					if (shape != null && !shape.Locked && (shape.InventoryOwner == 0 || (shape.InventoryOwnerType == 0 && shape.InventoryOwner == player.DbModel.Id || shape.InventoryOwnerType == 1 && shape.InventoryOwner == player.DbModel.Team)))
					{
						if (shape.Inventory != null)
						{
							targetId = shape.Id;
							targetInventory = shape.Inventory;
						}
						else if (shape.ShapeType == ColshapeType.LABORATORY_INPUT && shape.Id == player.DbModel.Team)
						{
							targetId = shape.Id;
							targetInventory = player.DbModel.LaboratoryInput;
						}
						else if (shape.ShapeType == ColshapeType.LABORATORY_OUTPUT && shape.Id == player.DbModel.Team)
						{
							targetId = shape.Id;
							targetInventory = player.DbModel.LaboratoryOutput;
						}
					}
				}
			}
			else
			{
				targetId = ((ClVehicle)player.Vehicle).DbModel!.Id;
				targetInventory = ((ClVehicle)player.Vehicle).DbModel!.GloveBox;
			}

			player.ShowComponent("Inventory", true, JsonConvert.SerializeObject(new InventoryDataModel(player.DbModel.Inventory, targetId, targetInventory, (targetPlayer == null ? -1 : giveItemTargetId))));
		}

		private async void UseItem(ClPlayer player, string eventKey, int slot, int amount)
		{
			await player.ShowComponent("Inventory", false);
			if (player.DbModel == null || player.IsInVehicle) return;

			await player.DbModel.Inventory.UseItem(player, slot, amount);
		}

		private async void GiveItem(ClPlayer player, string eventKey, int targetId, int slot, int amount)
		{
			var target = ClPlayer.LoggedIn.FirstOrDefault(x => x.Id == targetId);
			if (target == null || player.Position.Distance(target.Position) > 3) return;

			var item = player.DbModel.Inventory.Items.FirstOrDefault(x => x.Slot == slot);
			if (item == null) return;

			if(target.DbModel.Inventory.GetFreeSlots() < Math.Ceiling((decimal)item.Amount / item.Model.MaxAmount) || target.DbModel.Inventory.MaxWeight - target.DbModel.Inventory.GetInventoryWeight() < item.Amount * item.Model.Weight)
			{
				await player.ShowComponent("Inventory", false);
				await player.Notify("FEHLER", "Diese Person hat nicht genug Platz!", NotificationType.ERROR);
				return;
			}

			player.PlayAnimation(AnimationType.GIVE_ITEM);
			if (player.DbModel.Inventory.RemoveItem(slot, amount))
			{
				target.DbModel.Inventory.AddItem(item.Model, amount);
				await player.Notify("INFORMATION", $"Du hast jemandem {amount}x {item.Model.Name} gegeben.", NotificationType.INFO);
				await target.Notify("INFORMATION", $"Jemand hat dir {amount}x {item.Model.Name} gegeben.", NotificationType.INFO);
			}
		}

		private void ThrowItem(ClPlayer player, string eventKey, int slot, int amount)
		{
			player.PlayAnimation(AnimationType.GIVE_ITEM);
			player.DbModel.Inventory.RemoveItem(slot, amount);
		}

		private void MoveAll(ClPlayer player, string eventKey, int oldSlot, int newSlot)
		{
			if (!player.DbModel.Inventory.MoveAllToSlot(oldSlot, newSlot))
			{
				player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				player.ShowComponent("Inventory", false);
			}
		}

		private void MoveAmount(ClPlayer player, string eventKey, int oldSlot, int newSlot, int amount)
		{
			if (amount < 1) return;

			if (!player.DbModel.Inventory.MoveAmountToSlot(oldSlot, newSlot, amount))
			{
				player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				player.ShowComponent("Inventory", false);
			}
		}

		private async void ContainerMoveAll(ClPlayer player, string eventKey, int containerId, int containerType, int oldSlot, int newSlot)
		{
			var targetInventory = await GetInventory(player, containerId, containerType);
			if (targetInventory == null || !targetInventory.MoveAllToSlot(oldSlot, newSlot))
			{
				await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				await player.ShowComponent("Inventory", false);
				return;
			}
		}

		private async void ContainerMoveAmount(ClPlayer player, string eventKey, int containerId, int containerType, int oldSlot, int newSlot, int amount)
		{
			if (amount < 1) return;

			var targetInventory = await GetInventory(player, containerId, containerType);
			if (targetInventory == null || !targetInventory.MoveAmountToSlot(oldSlot, newSlot, amount))
			{
				await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				await player.ShowComponent("Inventory", false);
				return;
			}
		}

		private async void MoveAcrossContainer(ClPlayer player, string eventKey, int containerId, int containerType, int oldSlot, int newSlot, bool type)
		{
			var container = await GetInventory(player, containerId, containerType);
			if (container == null)
			{
				await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				await player.ShowComponent("Inventory", false);
				return;
			}

			if(container.Type == InventoryType.PLAYER && type)
			{
				await player.ShowComponent("Inventory", false);
				return;
			}

			var rootInventory = (type ? player.DbModel.Inventory : container)!;
			var targetInventory = (type ? container : player.DbModel.Inventory)!;

			var item = rootInventory.Items.FirstOrDefault(x => x.Slot == oldSlot);
			if (item == null) return;

			var targetItem = targetInventory.Items.FirstOrDefault(x => x.Slot == newSlot);

			if(targetInventory.Type == InventoryType.TEAM && !player.DbModel.TeamStoragePermission)
			{
				await player.Notify("FRAKTION", "Du hast keine Lagerrechte!", NotificationType.ERROR);
				return;
			}

			if (targetItem != null)
			{
				if (item.Model.Id == targetItem.Model.Id)
				{
					var diff = targetItem.Model.MaxAmount - targetItem.Amount;
					if (targetInventory.GetInventoryWeight() + diff * targetItem.Model.Weight > targetInventory.MaxWeight) return;

					if (item.Amount <= diff)
					{
						rootInventory.Items.Remove(item);
						targetItem.Amount += item.Amount;
						return;
					}

					item.Amount -= diff;
					targetItem.Amount += diff;
					player.PlayAnimation(AnimationType.GIVE_ITEM);
					return;
				}

				if (rootInventory.GetInventoryWeight() - (item.Model.Weight * item.Amount) + (targetItem.Amount * targetItem.Model.Weight) > rootInventory.MaxWeight)
				{
					await player.ShowComponent("Inventory", false);
					await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
					return;
				}

				if (targetInventory.GetInventoryWeight() - (targetItem.Amount * targetItem.Model.Weight) + (item.Model.Weight * item.Amount) > targetInventory.MaxWeight)
				{
					await player.ShowComponent("Inventory", false);
					await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
					return;
				}

				var newItem = new ItemModel(targetItem.Model, targetItem.Amount, item.Slot);
				var newTargetItem = new ItemModel(item.Model, item.Amount, targetItem.Slot);

				rootInventory.Items.Remove(item);
				targetInventory.Items.Remove(targetItem);
				rootInventory.Items.Add(newItem);
				targetInventory.Items.Add(newTargetItem);
				player.PlayAnimation(AnimationType.GIVE_ITEM);
				return;
			}

			if (targetInventory.GetInventoryWeight() + item.Model.Weight * item.Amount > targetInventory.MaxWeight)
			{
				await player.ShowComponent("Inventory", false);
				await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				return;
			}

			if (rootInventory.Items.Remove(item))
			{
				targetInventory.Items.Add(new ItemModel(item.Model, item.Amount, newSlot));
				player.PlayAnimation(AnimationType.GIVE_ITEM);
			}
		}

		private async void MoveAmountAcrossContainer(ClPlayer player, string eventKey, int containerId, int containerType, int oldSlot, int newSlot, int amount, bool type)
		{
			var container = await GetInventory(player, containerId, containerType);
			if (container == null)
			{
				await player.ShowComponent("Inventory", false);
				await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				return;
			}

			if (container.Type == InventoryType.PLAYER && type)
			{
				await player.ShowComponent("Inventory", false);
				return;
			}

			var rootInventory = type ? player.DbModel.Inventory : container;
			var targetInventory = type ? container : player.DbModel.Inventory;

			var item = rootInventory.Items.FirstOrDefault(x => x.Slot == oldSlot);
			var targetItem = targetInventory.Items.FirstOrDefault(x => x.Slot == newSlot);
			if (targetItem != null || item == null || item.Amount < amount) return;

			if (targetInventory.GetInventoryWeight() + (item.Model.Weight * amount) > targetInventory.MaxWeight)
			{
				await player.ShowComponent("Inventory", false);
				await player.Notify("Inventar", "Ein Fehler ist aufgetreten!", NotificationType.ERROR);
				return;
			}

			item.Amount -= amount;
			targetInventory.Items.Add(new ItemModel(item.Model, amount, newSlot));
			if (item.Amount < 1) rootInventory.Items.Remove(item);
			player.PlayAnimation(AnimationType.GIVE_ITEM);
		}

		private async Task<InventoryModel?> GetInventory(ClPlayer player, int containerId, int containerType)
		{
			InventoryModel? targetInventory = null;
			var shape = player.CurrentShape;

			switch ((InventoryType)containerType)
			{
				case InventoryType.PLAYER:
					targetInventory = ClPlayer.All.FirstOrDefault(x => x.DbModel?.Id == containerId)?.DbModel.Inventory;
					break;
				case InventoryType.VEHICLE_TRUNK:
					targetInventory = ClVehicle.All.FirstOrDefault(x => x.DbModel?.Id == containerId)?.DbModel!.Trunk;
					break;
				case InventoryType.VEHICLE_GLOVEBOX:
					targetInventory = ClVehicle.All.FirstOrDefault(x => x.DbModel?.Id == containerId)?.DbModel!.GloveBox;
					break;
				case InventoryType.DROP_BOX_1:
					targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.DROP_BOX_1)?.Inventory;
					break;
				case InventoryType.DROP_BOX_2:
					targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.DROP_BOX_2)?.Inventory;
					break;
				case InventoryType.DROP_BOX_3:
					targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.DROP_BOX_3)?.Inventory;
					break;
				case InventoryType.TEAM:
					if (shape == null || shape.ShapeType != ColshapeType.TEAM_INVENTORY) break;

					var team = await _teamService.GetTeam(shape.Id);
					if (team == null) break;

					targetInventory = team.Inventories[shape.StorageInventoryId].Inventory;
					break;
				case InventoryType.STORAGE:
					if (shape == null || shape.ShapeType != ColshapeType.STORAGE) break;

                    var storage = await _storageService.GetStorage(containerId);
                    if (storage == null) break;

					targetInventory = storage.Inventories[shape.StorageInventoryId];
					break;
				case InventoryType.BANK_ROBBERY_LOOT:
					targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.BANK_ROBBERY_LOOT)?.Inventory;
					break;
				case InventoryType.JEWELERY_LOOT:
					targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.JEWELERY_LOOT)?.Inventory;
					break;
				case InventoryType.LABORATORY_INPUT:
					targetInventory = player.DbModel.LaboratoryInput;
					break;
				case InventoryType.LABORATORY_OUTPUT:
					targetInventory = player.DbModel.LaboratoryOutput;
					break;
				case InventoryType.LABORATORY_FUEL:
					targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.LABORATORY_FUEL)?.Inventory;
					break;
                case InventoryType.LABORATORY_ROB:
                    targetInventory = ClShape.Get(x => x.Id == containerId && x.ShapeType == ColshapeType.LABORATORY_ROB)?.Inventory;
                    break;
                case InventoryType.HOUSE_INVENTORY:
					if (shape == null || shape.ShapeType != ColshapeType.HOUSE_INVENTORY) break;

                    var house = await _houseService.GetHouse(shape.Id);
					if (house == null) break;

					targetInventory = house.Inventory;
                    break;
            }

			return targetInventory;
		}
	}
}