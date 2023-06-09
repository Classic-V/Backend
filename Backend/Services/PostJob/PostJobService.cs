using Backend.Controllers.PostJob.Interface;
using Backend.Services.PostJob.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.PostJob;

public class PostJobService : IPostJobService
{
    public List<PostJobModel> PostJobs { get; private set; }

    public PostJobService()
    {
        var ctx = new DBContext();
        PostJobs = new List<PostJobModel>(ctx.PostJob);
    }

    public Task<PostJobModel> GetJob(int id)
    {
        return Task.FromResult(PostJobs.FirstOrDefault(x => x.Id == id))!;
    }

    public async Task AddJob(PostJobModel model)
    {
        PostJobs.Add(model);

        var ctx = new DBContext();
        ctx.PostJob.Add(model);
        await ctx.SaveChangesAsync();
    }

    public async Task DeleteJob(PostJobModel model)
    {
        PostJobs.Remove(model);

        var ctx = new DBContext();
        ctx.PostJob.Remove(model);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateJob(PostJobModel model)
    {
        var ctx = new DBContext();
        ctx.PostJob.Update(model);
        await ctx.SaveChangesAsync();
    }
}