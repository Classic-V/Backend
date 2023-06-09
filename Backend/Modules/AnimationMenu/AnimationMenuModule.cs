using Backend.Controllers.Animation.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.AnimationMenu;

public class AnimationMenuModule : Module<AnimationMenuModule>
{
    private readonly IAnimationController _animationController;

    public AnimationMenuModule(IAnimationController animationController, IEventController eventController) : base("AnimationMenu")
    {
        _animationController = animationController;

        eventController.OnClient("Server:NMenu:Open", Open);
		eventController.OnClient<int, int>("Server:NMenu:SaveAnimation", SaveAnimation);
        eventController.OnClient<int>("Server:NMenu:PlayAnimation", PlayAnimation);
		eventController.OnClient("Server:NMenu:OpenSelectSlot", OpenSelectSlot);
		eventController.OnClient<int>("Server:NMenu:OpenSelectCategory", OpenSelectCategory);
		eventController.OnClient<int, int>("Server:NMenu:OpenSelectAnimation", OpenSelectAnimation);
	}

    private async void Open(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null!) return;

        await player.ShowComponent("NMenu", true, JsonConvert.SerializeObject(player.DbModel.FavoriteAnimations));
    }

	private async void SaveAnimation(ClPlayer player, string eventKey, int animationId, int slot)
    {
	   var animationSlot = player.DbModel.FavoriteAnimations[slot];
       if (animationSlot == null) return;

		var animation = await _animationController.GetAnimation(animationId);
       if (animation == null) return;

		animationSlot.Id = animation.Id;
       animationSlot.Label = animation.Name;

		await player.Notify("Animation", "Animation abgespeichert!", NotificationType.INFO);
    }

    private async void PlayAnimation(ClPlayer player, string eventKey, int animationId)
    {
        if (player.DbModel == null ||animationId < -1) return;

        if (animationId == -1)
        {
            player.StopAnimation();
            return;
        }

        if(animationId == 0)
        {
            OpenSelectSlot(player, "");
			return;
        }

        var animation = await _animationController.GetAnimation(animationId);
        if (animation == null) return;

        player.StopAnimation();
        player.PlayAnimation(animation.AnimationDictionary, animation.AnimationName, animation.AnimationFlag);
    }

    private async void OpenSelectSlot(ClPlayer player, string eventKey)
    {
		var items = new List<ClientNativeMenuItem>();
        for(var i = 0; i < 10; i++)
        {
            items.Add(new ClientNativeMenuItem($"Slot #{i + 1} belegen", false, "Server:NMenu:OpenSelectCategory", i));
        }

		var menu = new ClientNativeMenu("Animation belegen", items);
		await player.ShowNativeMenu(true, menu);
	}

	private async void OpenSelectCategory(ClPlayer player, string eventKey, int slot)
	{
		var items = new List<ClientNativeMenuItem>
		{
			new ("Zurück", true, "Server:NMenu:OpenSelectSlot"),
			new ("Sitzen", true, "Server:NMenu:OpenSelectAnimation", slot, AnimationCategoryType.SIT),
			new ("Liegen", true, "Server:NMenu:OpenSelectAnimation", slot, AnimationCategoryType.LAY),
			new ("Gesten", true, "Server:NMenu:OpenSelectAnimation", slot, AnimationCategoryType.GESTURE),
			new ("Tanzen", true, "Server:NMenu:OpenSelectAnimation", slot, AnimationCategoryType.DANCE),
			new ("Tätigkeiten", true, "Server:NMenu:OpenSelectAnimation", slot, AnimationCategoryType.ACTIVITY),
			new ("Sport", true, "Server:NMenu:OpenSelectAnimation", slot, AnimationCategoryType.SPORTS),
		};

		var menu = new ClientNativeMenu("Animation belegen", items);
		await player.ShowNativeMenu(true, menu);
	}

	private async void OpenSelectAnimation(ClPlayer player, string eventKey, int slot, int category)
	{
		var items = new List<ClientNativeMenuItem>
		{
			new ("Zurück", true, "Server:NMenu:OpenSelectCategory", slot),
		};

		var categoryItems = await _animationController.GetAnimationCategory((AnimationCategoryType)category);
		if (categoryItems == null) return;

		categoryItems.ForEach(animation =>
		{
			items.Add(new ClientNativeMenuItem(animation.Name, true, "Server:NMenu:SaveAnimation", animation.Id, slot));
		});

		var menu = new ClientNativeMenu("Animation belegen", items);
		await player.ShowNativeMenu(true, menu);
	}
}