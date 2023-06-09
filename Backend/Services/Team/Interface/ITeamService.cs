using Backend.Utils.Models.Database;

namespace Backend.Services.Team.Interface
{
    public interface ITeamService
    {
        List<TeamModel> Teams { get; }
        Task<int> AddTeam(TeamModel team);
        Task<TeamModel?> GetTeam(int id);
        Task<TeamModel?> GetTeam(string name);
        Task UpdateTeam(TeamModel team);
        Task DeleteTeam(int id);
        Task UpdateTeams();

	}
}
