using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.Animation.Interface;

public interface IAnimationController
{
    Task<List<AnimationModel>> GetAnimations();
    Task<AnimationModel?> GetAnimation(int id);
}