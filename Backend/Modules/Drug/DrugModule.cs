using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Drug
{
	public class DrugModule : Module<DrugModule>, IEventIntervalMinute
	{
		public DrugModule() : base("Drug")
		{
		}

		public Task OnEveryMinute()
		{
			foreach(var player in ClPlayer.All)
			{
				if (player.DbModel == null || !player.DrugState.AnyDrugActive) continue;

				if(player.DrugState.MDMA.Active && player.DrugState.MDMA.UsedTime.AddMinutes(20) < DateTime.Now)
				{
					player.DrugState.MDMA.Active = false;
					player.Emit("Client:PlayerModule:SetRunSpeedMultiplier", Math.Clamp(player.DbModel.Strength, 80, 110) / 100);
				}

				if (player.DrugState.Meth.Active && player.DrugState.Meth.UsedTime.AddMinutes(20) < DateTime.Now)
				{
					player.DrugState.Meth.Active = false;
				}

				if (player.DrugState.Ecstasy.Active && player.DrugState.Ecstasy.UsedTime.AddMinutes(20) < DateTime.Now)
				{
					player.DrugState.Ecstasy.Active = false;
				}

				if (player.DrugState.Heroin.Active && player.DrugState.Heroin.UsedTime.AddMinutes(20) < DateTime.Now)
				{
					player.DrugState.Heroin.Active = false;
					player.MaxHealth = 200;
				}
			}

			return Task.CompletedTask;
		}
	}
}