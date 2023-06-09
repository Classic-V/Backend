using AltV.Net.Data;

namespace Backend.Utils.Models.Medic
{
	public class HospitalModel
	{
		public Position EntryPosition { get; set; }
		public List<PositionModel> BedPositions { get; set; }

		public HospitalModel(Position entryPosition, List<PositionModel> bedPositions)
		{
			EntryPosition = entryPosition;
			BedPositions = bedPositions;
		}
	}
}