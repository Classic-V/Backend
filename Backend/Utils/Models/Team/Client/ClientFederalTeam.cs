namespace Backend.Utils.Models.Team.Client
{
	public class ClientFederalTeam
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Warns { get; set; }
		public bool SWAT { get; set; }
		public int Money { get; set; }
		public List<TeamClientMemberData> Members { get; set; }
		public List<TeamClientBankData> BankHistory { get; set; }
		public TeamClientBusinessData Business { get; set; }

		public ClientFederalTeam(int id, string name, int warns, bool sWAT, int money, List<TeamClientMemberData> members, List<TeamClientBankData> bankHistory, TeamClientBusinessData business)
		{
			var _bankHistory = new List<TeamClientBankData>();

			for(var i = 0; i < bankHistory.Count; i++)
			{
				if (bankHistory.Count - i > 30) continue;
				_bankHistory.Add(bankHistory[i]);
			}

			Id = id;
			Name = name;
			Warns = warns;
			SWAT = sWAT;
			Money = money;
			Members = members;
			BankHistory = _bankHistory;
			Business = business;
		}
	}
}