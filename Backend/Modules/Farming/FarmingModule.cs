using Backend.Controllers.Event.Interface;
using Backend.Controllers.Farming.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using AltV.Net.Elements.Entities;
using System.Numerics;
using Backend.Data;

namespace Backend.Modules.Farming
{
	public class FarmingModule : Module<FarmingModule>, IEventIntervalFiveSeconds, IEventColshape
	{
		private readonly IFarmingController _farmingController;

		public FarmingModule(IEventController eventController, IFarmingController farmingController) : base("Farming")
		{
			_farmingController = farmingController;

			eventController.OnClient("Server:Farming:Start", StartFarming);
			eventController.OnClient("Server:Farming:Stop", StopFarming);
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.FARMING_SPOT) return Task.CompletedTask;

			var player = (ClPlayer)entity;

			if(entered) player.SetInteraction(Interactions.KEY_E, "FARMING");

			if (!player.IsFarming && entered) return Task.CompletedTask;
			player.IsFarming = false;
			player.SetPlayerFarming(false);
			player.StopAnimation();

			return Task.CompletedTask;
		}

		public Task OnEveryFiveSeconds()
		{
			_farmingController.OnTick();
			return Task.CompletedTask;
		}

		private void StartFarming(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null) return;

			var shape = ClShape.Get(x => x.ShapeType == ColshapeType.FARMING_SPOT && x.Position.Distance(player.Position) <= x.Size);
			if (shape == null) return;

			_farmingController.StartFarming(player, shape.Id, shape.FarmingIndex);
		}

		private void StopFarming(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || !player.IsFarming) return;

			player.IsFarming = false;
			player.CurrentFarmingSpot = 0;
			player.CurrentFarmingIndex = 0;
			player.SetPlayerFarming(false);
			player.StopAnimation();
		}
	}
}