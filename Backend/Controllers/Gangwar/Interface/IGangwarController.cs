using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Gangwar.Interface
{
    public interface IGangwarController
    {
        Task<bool> AttackGangwar(ClPlayer player, int id, int attackerId);
        Task EndGangwar(int id);
        Task UpdateGangwar(int id, int ownerId);
        Task<GangwarModel> GetGangwarModel(int id);
        Task<GangwarModel> GetRunningGangwarByTeam(int teamId);
        Task<bool> IsGangwarFull(int id, int teamId);
        Task FlagTick();
        Task OnJoin(ClPlayer player, int gwId);
        Task PlayerQuitGangwar(ClPlayer player);
        Task PlayerDeath(ClPlayer player, IEntity? killer);
	}
}
