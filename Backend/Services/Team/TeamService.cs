using Backend.Services.Team.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Team
{
    public class TeamService : ITeamService
    {
        public List<TeamModel> Teams { get; private set; } = new();

        public TeamService()
        {
            LoadTeams();
        }

        private async void LoadTeams()
        {
            await Task.Delay(15000);
			using var ctx = new DBContext();
			Teams = new List<TeamModel>(ctx.Teams);
		}

        public async Task<int> AddTeam(TeamModel team)
        {
            await using var ctx = new DBContext();

			Teams.Add(team);
			await ctx.Teams.AddAsync(team);
            await ctx.SaveChangesAsync();

            return team.Id;
        }

        public Task<TeamModel?> GetTeam(int id)
        {
            return Task.FromResult(Teams.FirstOrDefault(x => x.Id == id));
        }

        public Task<TeamModel?> GetTeam(string name)
        {
            return Task.FromResult(Teams.FirstOrDefault(x => x.Name == name));
        }

        public async Task UpdateTeam(TeamModel team)
        {
            await using var ctx = new DBContext();
            ctx.Teams.Update(team);
            await ctx.SaveChangesAsync();
        }

		public async Task UpdateTeams()
		{
			await using var ctx = new DBContext();
			ctx.Teams.UpdateRange(Teams);
			await ctx.SaveChangesAsync();
		}

		public async Task DeleteTeam(int id)
        {
            var teamModel = await GetTeam(id);
            if (teamModel == null) return;

            await using var ctx = new DBContext();
            ctx.Teams.Remove(teamModel);
            await ctx.SaveChangesAsync();
        }
    }
}
