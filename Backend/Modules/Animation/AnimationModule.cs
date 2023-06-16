using System.Reflection.Metadata;
using Backend.Controllers.Animation.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;
using Object = Backend.Utils.Models.Object;

namespace Backend.Modules.Animation;

public class AnimationModule : Module<AnimationModule>
{
    private readonly IAnimationController _animationController;

    public AnimationModule(IAnimationController animationController, IEventController eventController) : base("Animation")
    {
        _animationController = animationController;

        eventController.OnClient("Server:AnimationMenu:Open", Open);
        eventController.OnClient<int>("Server:AnimationMenu:PlayAnimation", PlayAnimation);
    }

    private async void Open(ClPlayer player, string eventKey)
    {
        if (player == null! || player.DbModel == null! || !player.DbModel.Alive || player.Interaction ||
            player.IsFarming) return;

        var items = new List<ClientNativeMenuItem>();

        var animations = await _animationController.GetAnimations();
        
        foreach (var animationModel in animations.OrderBy(x => x.Name))
        {
            items.Add(new ClientNativeMenuItem(animationModel.Name) { CallbackEvent = "Server:AnimationMenu:PlayAnimation", CallbackArgs = new object[] { animationModel.Id }});
        }
		
        var menu = new ClientNativeMenu("Animation spielen", items);
        await player.ShowNativeMenu(true, menu);
    }

    private async void PlayAnimation(ClPlayer player, string eventKey, int animationId)
    {
        if (player == null! || player.DbModel == null! || !player.DbModel.Alive || player.Interaction ||
            player.IsFarming) return;

        var animation = await _animationController.GetAnimation(animationId);
        if (animation == null) return;

        player.StopAnimation();
        player.PlayAnimation(animation.AnimationDictionary, animation.AnimationName, animation.AnimationFlag);
    }
}