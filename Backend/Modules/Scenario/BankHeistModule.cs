using AltV.Net.Data;
using Backend.Controllers.BankRobbery.Interface;
using Backend.Utils.Models;

namespace Backend.Modules.Scenario
{
	public class BankHeistModule : Module<BankHeistModule>
	{
		public static Position VaultPosition { get; } = new Position(253.22f, 228.59f, 101.66f);
		public static Position VaultEnterPosition { get; } = new Position(253.97f, 225.23f, 101.86f);
		public static Position VaultExitPosition { get; } = new Position(253.30f, 223.23f, 101.66f);
		public static List<Position> VaultPositions { get; } = new()
		{
			new Position(257.35f, 214.48f, 101.66f),
			new Position(259.29f, 213.71f, 101.66f),
			new Position(262.41f, 212.58f, 101.66f),
			new Position(264.36f, 211.91f, 101.66f),
			new Position(266.00f, 213.42f, 101.66f),
			new Position(265.67f, 215.74f, 101.66f),
			new Position(263.77f, 216.42f, 101.66f),
			new Position(260.59f, 217.60f, 101.66f),
			new Position(258.52f, 218.34f, 101.66f)
		};

		private readonly IBankHeistController _bankHeistController;

		public BankHeistModule(IBankHeistController bankHeistController) : base("BankHeist")
		{
			_bankHeistController = bankHeistController;
		}
	}
}