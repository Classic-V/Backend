using System.Reflection.Metadata;
using Backend.Controllers.Animation.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.Animation;

public class AnimationModule : Module<AnimationModule>
{
    private readonly IAnimationController _animationController;

    public AnimationModule(IAnimationController animationController, IEventController eventController) : base("Animation")
    {
        _animationController = animationController;

        eventController.OnClient("Server:AnimationMenu:Open", Open);
        eventController.OnClient<int>("Server:AnimationMenu:OpenCategory", OpenCategory);
        eventController.OnClient<int>("Server:AnimationMenu:PlayAnimation", PlayAnimation);
    }

    private async void Open(ClPlayer player, string eventKey)
    {
        if (player == null! || player.DbModel == null! || !player.DbModel.Alive || player.Interaction ||
            player.IsFarming) return;

        var items = new List<ClientNativeMenuItem>
        {
            new ("Sitzen", true, "Server:AnimationMenu:OpenCategory", AnimationCategoryType.SIT),
            new ("Liegen", true, "Server:AnimationMenu:OpenCategory", AnimationCategoryType.LAY),
            new ("Gesten", true, "Server:AnimationMenu:OpenCategory", AnimationCategoryType.GESTURE),
            new ("Tanzen", true, "Server:AnimationMenu:OpenCategory", AnimationCategoryType.DANCE),
            new ("Tätigkeiten", true, "Server:AnimationMenu:OpenCategory", AnimationCategoryType.ACTIVITY),
            new ("Sport", true, "Server:AnimationMenu:OpenCategory", AnimationCategoryType.SPORTS),
        };

        var menu = new ClientNativeMenu("Animations Menu", items);
        await player.ShowNativeMenu(true, menu);
    }

    private async void OpenCategory(ClPlayer player, string eventKey, int categoryId)
    {
        if (player == null! || player.DbModel == null! || !player.DbModel.Alive || player.Interaction ||
            player.IsFarming) return;

        var items = new List<ClientNativeMenuItem>();
        items.Add(new ClientNativeMenuItem("Zurück", false, "Server:AnimationMenu:Open"));

        var categoryItems = await _animationController.GetAnimationCategory((AnimationCategoryType)categoryId);
        if (categoryItems == null!) return;

        categoryItems.ForEach(animation =>
        {
            items.Add(new ClientNativeMenuItem(animation.Name, false, "Server:AnimationMenu:PlayAnimation", animation.Id));
        });

        AnimationCategoryType type = (AnimationCategoryType)categoryId;

        var title = type == AnimationCategoryType.SIT ? "Sitzen" :
            type == AnimationCategoryType.LAY ? "Liegen" :
            type == AnimationCategoryType.GESTURE ? "Gesten" :
            type == AnimationCategoryType.DANCE ? "Tanzen" :
            type == AnimationCategoryType.ACTIVITY ? "Tätigkeiten" :
            type == AnimationCategoryType.SPORTS ? "Sport" : "error";

        var menu = new ClientNativeMenu($"Animations Menu: {title}", items);
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