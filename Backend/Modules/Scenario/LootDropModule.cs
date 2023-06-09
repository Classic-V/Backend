using AltV.Net;
using Backend.Controllers.Drop.Interface;
using Backend.Data;
using Backend.Utils;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Streamer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Scenario
{
	public class LootDropModule : Module<LootDropModule>, IEventIntervalMinute
	{
		private readonly IDropController _dropController;

		public LootDropModule(IDropController dropController) : base("LootDrop")
		{
			_dropController = dropController;
		}

		public async Task OnEveryMinute()
		{
			await _dropController.Tick();
		}
	}
}