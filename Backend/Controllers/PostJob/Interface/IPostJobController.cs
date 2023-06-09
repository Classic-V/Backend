using Backend.Utils.Models.Database;

namespace Backend.Controllers.PostJob.Interface;

public interface IPostJobController
{
    void LoadPostJob(PostJobModel model);
    Task<PostJobModel> GetJob(int id);
}