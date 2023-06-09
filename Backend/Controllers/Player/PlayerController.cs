using Backend.Controllers.Player.Interface;
using Backend.Services.Account.Interface;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Player
{
	public class PlayerController : IPlayerController
	{
		private readonly IAccountService _accountService;

		public PlayerController(IAccountService accountService)
		{
			_accountService = accountService;
		}

		public async Task SavePlayer(ClPlayer player)
		{
			if (player.DbModel == null) return;

			await player.Update();
			await _accountService.UpdateAccount(player.DbModel);
		}
	}
}