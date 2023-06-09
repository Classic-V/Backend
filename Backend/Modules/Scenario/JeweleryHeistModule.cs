using AltV.Net.Data;
using Backend.Controllers.BankRobbery.Interface;
using Backend.Controllers.JeweleryHeist.Interface;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.Scenario
{
	public class JeweleryHeistModule : Module<JeweleryHeistModule>, IEventIntervalMinute
	{
		public static Position Position { get; } = new Position(-631.10f, -229.92f, 38.04f);
		public static List<Position> LootPositions { get; } = new()
		{
			 new Position(-627.45f, -233.53f, 38.04f),
			 new Position(-626.33f, -235.09f, 38.04f),
			 new Position(-626.13f, -238.14f, 38.04f),
			 new Position(-622.97f, -233.07f, 38.04f),
			 new Position(-624.52f, -230.92f, 38.04f),
			 new Position(-624.25f, -227.93f, 38.04f),
			 new Position(-621.08f, -228.58f, 38.04f),
			 new Position(-620.03f, -227.10f, 38.04f),
			 new Position(-618.06f, -229.91f, 38.04f),
			 new Position(-619.58f, -230.63f, 38.04f),
			 new Position(-619.68f, -233.49f, 38.04f)
		};

		private readonly IJeweleryHeistController _jeweleryHeistController;

		public JeweleryHeistModule(IJeweleryHeistController jeweleryHeistController) : base("JeweleryHeist")
		{
			_jeweleryHeistController = jeweleryHeistController;
		}

		public Task OnEveryMinute()
		{
			if (_jeweleryHeistController.IsBeingRobbed || _jeweleryHeistController.HasBeenAttacked || IsAnyPlayerNearJewelery()) return Task.CompletedTask;

			_jeweleryHeistController.StopHeist();

			return Task.CompletedTask;
		}

		private bool IsAnyPlayerNearJewelery()
		{
			return ClPlayer.All.FirstOrDefault(x => x.Position.Distance(Position) < 15) != null;
			return false;
		}
	}
}