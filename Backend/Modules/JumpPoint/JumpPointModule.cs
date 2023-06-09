using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.JumpPoint;
using Backend.Controllers.JumpPoint.Interface;
using Backend.Data;
using Backend.Services.JumpPoint.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.JumpPoint
{
	public class JumpPointModule : Module<JumpPointModule>, IEventColshape
	{
		private readonly IJumpPointController _jumpPointController;
		private readonly IJumpPointService _jumpPointService;

		public JumpPointModule(IEventController eventController, IJumpPointController jumpPointController, IJumpPointService jumpPointService) : base("JumpPoint")
		{
			_jumpPointController = jumpPointController;
			_jumpPointService = jumpPointService;

			eventController.OnClient("Server:JumpPoint:Lock", LockJumpPoint);
			eventController.OnClient("Server:JumpPoint:Enter", EnterJumpPoint);
		}

		private async void LockJumpPoint(ClPlayer player, string eventKey)
		{
			var shape = player.CurrentShape;
			if (shape == null || shape.ShapeType != ColshapeType.JUMP_POINT) return;

			await _jumpPointController.LockJumpPoint(player, shape.Id);
		}

		private async void EnterJumpPoint(ClPlayer player, string eventKey)
		{
			var shape = player.CurrentShape;
			if (shape == null || shape.ShapeType != ColshapeType.JUMP_POINT) return;

			await _jumpPointController.EnterJumpPoint(player, shape.Id, shape.JumpPointType);
		}

		public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.JUMP_POINT) return;

			var jumpPoint = await _jumpPointService.GetJumpPoint(shape.Id);
			if (jumpPoint == null) return;

			var player = (ClPlayer)entity;
			if(entered) player.SetInteraction(Interactions.KEY_E, Interactions.E_JUMPPOINT);
			player.SetInteraction(Interactions.KEY_L, entered ? Interactions.L_JUMPPOINT : Interactions.NONE);

			if (!shape.Locked && entered)
			{
				await player.Notify(jumpPoint.Name, $"Drücke E um durch den {(shape.JumpPointType ? "Eingang" : "Ausgang")} zu gehen.", NotificationType.INFO);
			}
		}
	}
}