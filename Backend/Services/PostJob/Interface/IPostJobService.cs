using Backend.Utils.Models.Database;

namespace Backend.Services.PostJob.Interface;

public interface IPostJobService
{
    List<PostJobModel> PostJobs { get; }
    Task<PostJobModel> GetJob(int id);
    Task AddJob(PostJobModel model);
    Task DeleteJob(PostJobModel model);
    Task UpdateJob(PostJobModel model);
}