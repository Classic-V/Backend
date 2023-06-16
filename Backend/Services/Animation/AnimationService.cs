using Backend.Services.Animation.Interface;
using Backend.Utils;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;

namespace Backend.Services.Animation;

public class AnimationService : IAnimationService
{
    public List<AnimationModel> Animations { get; private set; }

    public AnimationService()
    {
        var ctx = new DBContext();
        Animations = new List<AnimationModel>(ctx.Animations);
    }
    public Task<AnimationModel> GetAnimation(int id)
    {
        return Task.FromResult(Animations.FirstOrDefault(x => x.Id == id))!;
    }

    public async Task AddAnimation(AnimationModel model)
    {
        Animations.Add(model);

        var ctx = new DBContext();
        ctx.Animations.Add(model);
        await ctx.SaveChangesAsync();
    }
}