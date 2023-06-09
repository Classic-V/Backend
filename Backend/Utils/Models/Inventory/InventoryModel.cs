using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;

namespace Backend.Utils.Models.Inventory
{
	public class InventoryModel
	{
		public List<ItemModel> Items { get; set; }
		public float MaxWeight { get; set; }
		public int Slots { get; set; }
		public InventoryType Type { get; set; }

		public InventoryModel(InventoryType type)
		{
			Items = new();
			MaxWeight = 0;
			Slots = 0;
			Type = type;
		}

		public InventoryModel(float maxWeight, int slots, InventoryType type)
		{
			Items = new();
			MaxWeight = maxWeight;
			Slots = slots;
			Type = type;
		}

		public InventoryModel(List<ItemModel> items, float maxWeight, int slots, InventoryType type)
		{
			Items = items;
			MaxWeight = maxWeight;
			Slots = slots;
			Type = type;
		}

		public async Task UseItem(ClPlayer player, int slot, int amount)
		{
			if (player.Interaction) return;

			var item = Items.FirstOrDefault(e => e.Slot == slot);
			if (item == null || item.Amount < 1) return;

			await item.Use(player, slot, amount);
		}

		public bool CanCarryItem(ItemBase? itemModel, int amount)
		{
			for (var i = 0; i < Items.Count; i++)
			{
				if (Items[i].Model.Id == itemModel?.Id)
				{
					if (Items[i].Amount < Items[i].Model.MaxAmount)
					{
						amount -= Items[i].Model.MaxAmount - Items[i].Amount;
					}
				}
			}
			
			if (itemModel == null) return false;
			if (amount <= 0 || RoundUpValue((double)amount / itemModel.MaxAmount) <= GetFreeSlots()) return true;
			
			
			return false;
		}
		
		public double RoundUpValue(double value)
		{
			var result = Math.Round(value, 0);
			if (result < value)
			{
				result += Math.Pow(10, -0);
			}
			return result;
		}
		
		public bool AddItem(ItemBase? itemModel, int amount)
		{
			if (itemModel == null || GetInventoryWeight() + itemModel.Weight * amount > MaxWeight) return false;

			var items = Items;
			var amount2 = amount;
			if (itemModel.MaxAmount > 1)
			{
				for (var i = 0; i < items.Count; i++)
				{
					if (items[i].Model.Name == itemModel.Name)
					{
						if (items[i].Amount + amount2 > items[i].Model.MaxAmount)
						{
							var diff = items[i].Model.MaxAmount - items[i].Amount;
							items[i].Amount += diff;
							amount2 -= diff;
							continue;
						}

						items[i].Amount += amount2;
						amount2 = 0;
						break;
					}
				}
			}

			while (amount2 > 0)
			{
				var freeslot = GetFirstFreeSlot(items);
				if (freeslot == -1) return false;
				if (amount2 > itemModel.MaxAmount)
				{
					items.Add(new ItemModel(itemModel, itemModel.MaxAmount, freeslot));
					amount2 -= itemModel.MaxAmount;
					continue;
				}

				items.Add(new ItemModel(itemModel, amount2, freeslot));
				break;
			}

			Items = items;
			return true;
		}

		public bool RemoveItem(int slot, int amount)
		{
			var item = Items.FirstOrDefault(e => e.Slot == slot);
			if (item == null || item.Amount < amount) return false;
			item.Amount -= amount;
			if (item.Amount < 1) Items.Remove(item);
			return true;
		}

		public bool RemoveItem(ItemBase model, int amount)
		{
			if (amount > Items.Where(x => x.Model.Id == model.Id).Sum(x => x.Amount)) return false;

			var amount2 = amount;
			for(var i = 0; i < Slots; i++)
			{
				var item = Items.FirstOrDefault(x => x.Slot == i);
				if (item == null || item.Model.Id != model.Id) continue;

				if(amount2 >= item.Amount)
				{
					amount2 -= item.Amount;
					Items.Remove(item);
					continue;
				}

				item.Amount -= amount2;
				break;
			}

			return true;
		}

		public bool MoveAllToSlot(int oldSlot, int newSlot)
		{
			var movedItem = Items.FirstOrDefault(e => e.Slot == oldSlot);
			var secondItem = Items.FirstOrDefault(e => e.Slot == newSlot);
			if (movedItem != null)
			{
				if (secondItem == null)
				{
					movedItem.Slot = newSlot;
					return true;
				}

				if (secondItem.Model.Name != movedItem.Model.Name)
				{
					movedItem.Slot = newSlot;
					secondItem.Slot = oldSlot;
				}
				else
				{
					var freeSpace = secondItem.Model.MaxAmount - secondItem.Amount;
					if (freeSpace < 1)
					{
						return true;
					}
					else
					{
						if (freeSpace <= movedItem.Amount)
						{
							secondItem.Amount += freeSpace;
							movedItem.Amount -= freeSpace;
							if (movedItem.Amount == 0)
								Items.Remove(movedItem);
						}
						else
						{
							secondItem.Amount += movedItem.Amount;
							Items.Remove(movedItem);
						}
					}
				}

				return true;
			}

			return false;
		}

		public bool MoveAmountToSlot(int oldSlot, int newSlot, int amount)
		{
			var item = Items.FirstOrDefault(e => e.Slot == oldSlot);
			if (item == null || item.Amount <= amount || Items.FirstOrDefault(e => e.Slot == newSlot) != null) return false;
			item.Amount -= amount;
			Items.Add(new ItemModel(item.Model, amount, newSlot));
			return true;
		}

		private int GetFirstFreeSlot(List<ItemModel> items)
		{
			for (var i = 1; i <= Slots; i++)
			{
				var r = items.FirstOrDefault(e => e.Slot == i);
				if (r == null || r.Amount < 1)
					return i;
			}

			return -1;
		}

		public int GetFreeSlots()
		{
			var slots = 0;
			for (var i = 1; i <= Slots; i++)
			{
				var r = Items.FirstOrDefault(e => e.Slot == i);
				if (r == null || r.Amount < 1)
					slots++;
			}

			return slots;
		}

		public bool HasItem(ItemBase model, out ItemModel? item)
		{
			item = Items.FirstOrDefault(x => x.Model.Name == model.Name);
			return item != null;
		}

		public bool HasItems(ItemBase model, int amount)
		{
			return Items.Where(x => x.Model.Id == model.Id).Sum(x => x.Amount) >= amount;
		}

		public float GetInventoryWeight()
		{
			float result = 0;
			for (var i = 0; i < Items.Count; i++)
				result += Items[i].Model.Weight * Items[i].Amount;
			return result;
		}

		public int GetItemsCount(ItemBase model)
		{
			var items = Items.Where(x => x.Model.Name == model.Name);
			var count = 0;
			foreach (var item in items)
				count += item.Amount;

			return count;
		}
	}
}