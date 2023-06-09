namespace Backend.Utils.Models.Team.Client
{
	public class ClientTeam
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Warns { get; set; }
		public int Gws { get; set; }
		public int Money { get; set; }
		public TeamStorageClientData Storage { get; set; }
		public List<TeamClientMemberData> Members { get; set; }
		public List<TeamClientBankData> BankHistory { get; set; }
		public TeamClientLaboratoryData Laboratory { get; set; }
		public TeamClientBusinessData Business { get; set; }

		public ClientTeam(int id, string name, int warns, int gws, int money, TeamStorageClientData storage, List<TeamClientMemberData> members, List<TeamClientBankData> bankHistory, TeamClientLaboratoryData laboratory, TeamClientBusinessData business)
		{
			var _bankHistory = new List<TeamClientBankData>();

			for (var i = 0; i < bankHistory.Count; i++)
			{
				if (bankHistory.Count - i > 30) continue;
				_bankHistory.Add(bankHistory[i]);
			}

			Id = id;
			Name = name;
			Warns = warns;
			Gws = gws;
			Money = money;
			Storage = storage;
			Members = members;
			BankHistory = _bankHistory;
			Laboratory = laboratory;
			Business = business;
		}
	}
}