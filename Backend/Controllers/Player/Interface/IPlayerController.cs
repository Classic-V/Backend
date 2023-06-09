using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Player.Interface
{
	public interface IPlayerController
	{
		Task SavePlayer(ClPlayer player);
	}
}