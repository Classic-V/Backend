using Backend.Utils.Models.Database;

namespace Backend.Services.JumpPoint.Interface
{
    public interface IJumpPointService
    {
        List<JumpPointModel> JumpPoints { get; }
        Task AddJumpPoint(JumpPointModel jumpPoint, bool inDatabase);
        Task<JumpPointModel?> GetJumpPoint(int  jumpPointId);
        Task RemoveLocalJumpPoint(int id);

	}
}
