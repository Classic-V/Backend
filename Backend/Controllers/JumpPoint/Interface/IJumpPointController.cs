using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.JumpPoint;

namespace Backend.Controllers.JumpPoint.Interface
{
    public interface IJumpPointController
    {
        Task CreateJumpPoint(JumpPointModel model, bool inDatabase);
        Task LockJumpPoint(ClPlayer player, int pointId);
        Task EnterJumpPoint(ClPlayer player, int pointId, bool enter);
        Task SetOwner(int pointId, List<int> playerAccess, List<int> factionAccess);
        Task CrackJumpPoint(ClPlayer player, ClShape shape);
	}
}
