using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.JumpPoint.Interface;
using Backend.Data;
using Backend.Services.JumpPoint.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.JumpPoint;

namespace Backend.Controllers.JumpPoint
{
    public class JumpPointController : IJumpPointController
    {
        private readonly IJumpPointService _jumpPointService;

        public JumpPointController(IJumpPointService jumpPointService)
        {
            _jumpPointService = jumpPointService;

            _jumpPointService.JumpPoints.ForEach(LoadPoint);
        }

        private void LoadPoint(JumpPointModel model)
        {
            var enter = (ClShape)Alt.CreateColShapeSphere(model.EntryPosition.Position, model.EntryPosition.Radius);
			enter.Id = model.Id;
			enter.ShapeType = ColshapeType.JUMP_POINT;
            enter.Size = model.EntryPosition.Radius;
			enter.Locked = model.Locked;
			enter.JumpPointType = true;
			enter.Dimension = model.EntryPosition.Dimension;

            var exit = (ClShape)Alt.CreateColShapeSphere(model.ExitPosition.Position, model.ExitPosition.Radius);
			exit.Id = model.Id;
			exit.ShapeType = ColshapeType.JUMP_POINT;
			exit.Size = model.ExitPosition.Radius;
			exit.Locked = model.Locked;
			exit.JumpPointType = false;
			exit.Dimension = model.ExitPosition.Dimension;
        }

        public async Task CrackJumpPoint(ClPlayer player, ClShape shape)
        {
            var point = await _jumpPointService.GetJumpPoint(shape.Id);
            if (point == null || !point.Crackable) return;

            if (point.OnCrack != null) point.OnCrack(player, point);
            player.PlayAnimation(AnimationType.WELDING);
            await player.StartInteraction(async () =>
            {
                if (player.Position.Distance(point.EntryPosition.Position) > 5) return;

				point.Locked = false;
                point.CrackedTime = DateTime.Now;
                await player.Notify("INFORMATION", "Du hast die Tür aufgebrochen!", NotificationType.SUCCESS);
			}, 300000);
        }

		public async Task CreateJumpPoint(JumpPointModel model, bool inDatabase)
        {
            await _jumpPointService.AddJumpPoint(model, inDatabase);

            LoadPoint(model);
        }

        public async Task LockJumpPoint(ClPlayer player, int pointId)
        {
            var jumpPoint = await _jumpPointService.GetJumpPoint(pointId);
			if (jumpPoint == null || !await IsOwner(jumpPoint, player.DbModel.Id, player.DbModel.Team) || !await IsNear(jumpPoint, player.Position)) return;

            if(jumpPoint.CrackedTime.AddMinutes(90) > DateTime.Now)
            {
                await player.Notify("INFORMATION", "Das Schloss ist noch kaputt.", NotificationType.ERROR);
                return;
            }

            jumpPoint.Locked = !jumpPoint.Locked;
            await UpdateShapeLocked(pointId, jumpPoint.Locked);
			if(jumpPoint.Locked) await player.Notify(jumpPoint.Name, "Abgeschlossen.", NotificationType.ERROR);
            else await player.Notify(jumpPoint.Name, "Aufgeschlossen.", NotificationType.SUCCESS);
		}

        public async Task EnterJumpPoint(ClPlayer player, int pointId, bool enter)
		{
			var jumpPoint = await _jumpPointService.GetJumpPoint(pointId);
			if (jumpPoint == null || jumpPoint.Locked || !await IsNear(jumpPoint, player.Position)) return;

			var targetPos = enter ? jumpPoint.ExitPosition : jumpPoint.EntryPosition;

            player.IsInInterior = enter;
            player.OutsideInteriorPosition = enter ? jumpPoint.EntryPosition.Position : Position.Zero;

            if (jumpPoint.OnInteract != null) jumpPoint.OnInteract(player, jumpPoint, enter);
			await player.SetPosition(targetPos.Position);
            await player.SetDimension(targetPos.Dimension);
		}

        public async Task SetOwner(int pointId, List<int> playerAccess, List<int> factionAccess)
        {
            var jumpPoint = await _jumpPointService.GetJumpPoint(pointId);
            if (jumpPoint == null) return;

            jumpPoint.PlayerAccessList = playerAccess;
            jumpPoint.FactionAccessList = factionAccess;
        }

        private Task<bool> IsOwner(JumpPointModel model, int playerId, int factionId)
        {
            return Task.FromResult(model.PlayerAccessList.Any(x => x == playerId) || model.FactionAccessList.Any(x => x == factionId));
        }

		private Task<bool> IsNear(JumpPointModel model, Position pos)
		{
            return Task.FromResult(pos.Distance(model.EntryPosition.Position) <= model.EntryPosition.Radius || pos.Distance(model.ExitPosition.Position) <= model.ExitPosition.Radius);
		}

        private Task UpdateShapeLocked(int id, bool state)
        {
            var shape = ClShape.Get(x => x.Id == id && x.ShapeType == ColshapeType.JUMP_POINT);
            if(shape != null) shape.Locked = state;
            return Task.CompletedTask;
        }
	}
}
