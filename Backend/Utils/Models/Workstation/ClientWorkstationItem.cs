using Backend.Modules.Inventory;

namespace Backend.Utils.Models.Workstation
{
	public class ClientWorkstationItem
	{
		public string Input { get; set; } = string.Empty;
		public int InputAmount { get; set; }
		public string Output { get; set; } = string.Empty;
		public int OutputAmount { get; set; }
		public int Price { get; set; }
		public int Duration { get; set; }
		public int Max { get; set; }

		public ClientWorkstationItem(WorkstationItemModel model)
		{
			var input = InventoryModule.GetItemBase(model.InputItem)!;
			var output = InventoryModule.GetItemBase(model.OutputItem)!;

			Input = input.Name;
			InputAmount = model.InputAmount;
			Output = output.Name;
			OutputAmount = model.OutputAmount;
			Price = model.Price;
			Duration = model.Duration;
			Max = model.Max;
		}
	}
}