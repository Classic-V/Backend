using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Team.Interface
{
	public interface ITeamController
	{
		void LoadTeam(TeamModel team);
		Task RobLaboratory(ClPlayer player);
	}
}