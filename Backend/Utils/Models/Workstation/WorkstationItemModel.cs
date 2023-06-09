namespace Backend.Utils.Models.Workstation
{
	public class WorkstationItemModel
	{
		public int InputItem { get; set; }
		public int InputAmount { get; set; }
		public int OutputItem { get; set; }
		public int OutputAmount { get; set; }
		public int Price { get; set; }
		public int Duration { get; set; }
		public int Max { get; set; }
	}
}