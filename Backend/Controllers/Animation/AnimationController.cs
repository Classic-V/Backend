using Backend.Controllers.Animation.Interface;
using Backend.Services.Animation.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Controllers.Animation;

public class AnimationController : IAnimationController
{
    private readonly IAnimationService _animationService;

    public AnimationController(IAnimationService animationService)
    {
        _animationService = animationService;
    }

    public Task<List<AnimationModel>> GetAnimations()
    {
        return Task.FromResult(_animationService.Animations);
    }

    public async Task<AnimationModel?> GetAnimation(int id)
    {
        return await _animationService.GetAnimation(id);
    }
}