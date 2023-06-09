using Backend.Modules.Inventory;
using Backend.Utils.Models.Player;

namespace Backend.Utils.Models.Workstation
{
	public class ClientWorkstationRunningItem
	{
		public string Output { get; set; } = string.Empty;
		public int OutputAmount { get; set; }
		public string Started { get; set; } = string.Empty;
		public int TimeLeft { get; set; }

		public ClientWorkstationRunningItem(PlayerWorkstationItemModel model)
		{
			var output = InventoryModule.GetItemBase(model.OutputItemId)!;

			Output = output.Name;
			OutputAmount = model.OutputItemAmount;
			Started = model.Date;
			TimeLeft = model.Duration - model.Ticks;
		}
	}
}