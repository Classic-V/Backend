using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.BankRobbery;
using Backend.Controllers.BankRobbery.Interface;
using Backend.Controllers.Drop.Interface;
using Backend.Controllers.JumpPoint.Interface;
using Backend.Data;
using Backend.Modules.Scenario;
using Backend.Services.Team;
using Backend.Utils.Enums;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Inventory.Items
{
	public class Schweissgeraet : ItemBase
	{
		public override int Id => 6;

		private readonly IDropController _dropController;
		private readonly IBankHeistController _bankHeistController;
		private readonly IJumpPointController _jumpPointController;

		public Schweissgeraet(IDropController dropController, IBankHeistController banHeistController, IJumpPointController jumpPointController) : base("Schweißgerät", 1f, 1)
		{
			_dropController = dropController;
			_bankHeistController = banHeistController;
			_jumpPointController = jumpPointController;
		}

		public override async Task<bool> Use(ClPlayer player, int slot, int amount)
		{
			var shape = player.CurrentShape;
			if(shape == null) return false;

			switch (shape.ShapeType)
			{
				case ColshapeType.DROP_MAIN:
					await DropInteraction(player);
					break;
				case ColshapeType.BANK_ROBBERY_MAIN:
					await BankRobberyInteraction(player);
					break;
				case ColshapeType.JUMP_POINT:
					await _jumpPointController.CrackJumpPoint(player, shape);
					break;
			}

			return true;
		}

		private async Task DropInteraction(ClPlayer player)
		{
			if (_dropController.IsDropOpen || player.DbModel.Team < 1) return;

			// Todo: add animation
			await player.StartInteraction(async () =>
			{
				if (_dropController.IsDropOpen) return;

				await _dropController.OpenDrop();
			}, 1800/*00*/);
		}

		private async Task BankRobberyInteraction(ClPlayer player)
		{
			if (!_bankHeistController.VaultLocked || player.DbModel.Team < 1) return;

			if (!await _bankHeistController.StartRobbery(player)) return;
			// Todo: add animation
			await player.StartInteraction(async () =>
			{
				if (!_bankHeistController.VaultLocked) return;

				await _bankHeistController.UnlockVault(player);
			}, 3000/*00*/);
		}
	}
}