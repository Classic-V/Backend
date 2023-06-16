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
		eventController.OnClient<int>("Server:NMenu:OpenSelectAnimation", OpenSelectAnimation);
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
            items.Add(new ClientNativeMenuItem($"Slot #{i + 1} belegen") { CallbackEvent = "Server:NMenu:OpenSelectAnimation", CallbackArgs = new object[] { i }});
        }

		var menu = new ClientNativeMenu("Animation belegen", items);
		await player.ShowNativeMenu(true, menu);
	}

	private async void OpenSelectAnimation(ClPlayer player, string eventKey, int slot)
	{
		var items = new List<ClientNativeMenuItem>();

		var animations = await _animationController.GetAnimations();
        
		foreach (var animationModel in animations.OrderBy(x => x.Name))
		{
			items.Add(new ClientNativeMenuItem(animationModel.Name) { Close = true, CallbackEvent = "Server:NMenu:SaveAnimation", CallbackArgs = new object[] { animationModel.Id, slot } });
		} 
		
		var menu = new ClientNativeMenu("Animation belegen", items, "Slot #" + (slot + 1));
		await player.ShowNativeMenu(true, menu);
	}
}