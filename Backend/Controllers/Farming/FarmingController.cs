using AltV.Net;
using Backend.Controllers.Farming.Interface;
using Backend.Services.Farming.Interface;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Enums;
using Backend.Utils.Streamer;
using AltV.Net.Data;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Backend.Modules.Inventory;

namespace Backend.Controllers.Farming
{
	public class FarmingController : IFarmingController
	{
		private readonly IFarmingService _farmingService;
		private readonly Random _random;

		public FarmingController(IFarmingService farmingService)
		{
			_farmingService = farmingService;
			_random = new Random();

			_farmingService.FarmingSpots.ForEach(LoadFarmingSpot);
		}

		public void LoadFarmingSpot(FarmingModel model)
		{
			for(var i = 0; i < model.Objects.Count; i++)
			{
				ClShape shape = (ClShape)Alt.CreateColShapeSphere(model.Objects[i].Position, 2f);
				shape.Id = model.Id;
				shape.ShapeType = ColshapeType.FARMING_SPOT;
				shape.Size = 2f;
				shape.FarmingIndex = i;
				shape.FarmingUsable = true;
				shape.FarmingDespawnTime = DateTime.Now;
				shape.FarmingObjectId = ObjectStreamer.AddObject(model.ObjectHash, new Position(model.Objects[i].Position.X, model.Objects[i].Position.Y, model.Objects[i].Position.Z-1), new Rotation(), false, false, 0);
			}
		}

		public void StartFarming(ClPlayer player, int spotId, int objectIndex)
		{
			var spot = _farmingService.GetFarmingSpot(spotId);
			if (spot == null || player.Position.Distance(spot.Objects[objectIndex].Position) > 3f) return;

			var neededItem = InventoryModule.GetItemBase(spot.NeededItem);
			if(neededItem != null && !player.DbModel.Inventory.HasItem(neededItem, out _))
			{
				player.Notify("Farming", $"Sieht so aus als würde dir das passende Werkzeug fehlen.", NotificationType.INFO);
				return;
			}

			player.SetPlayerFarming(true);
			player.IsFarming = true;
			player.CurrentFarmingSpot = spotId;
			player.CurrentFarmingIndex = objectIndex;
			player.PlayAnimation((AnimationType)spot.Animation);
		}

		public void OnTick()
		{
			foreach (var player in ClPlayer.All.Where(x => x.IsFarming))
			{
				var spot = _farmingService.GetFarmingSpot(player.CurrentFarmingSpot);
				if(spot == null) continue;
				var obj = spot.Objects[player.CurrentFarmingIndex];
				if(player.Position.Distance(obj.Position) > 2f)
				{
					player.IsFarming = false;
					player.SetPlayerFarming(false);
					player.StopAnimation();
					continue;
				}

				obj.Health--;
				var item = InventoryModule.GetItemBase(spot.GetItem);
				var amount = _random.Next(spot.MinItemGet, spot.MaxItemGet);
				if (item == null) continue;
;				player.DbModel.Inventory.AddItem(item, amount);
				player.Notify("FARMING", $"+{amount} {item.Name}", NotificationType.INFO);
			}

			CheckObjectsHealth();
		}

		public void CheckObjectsHealth()
		{
			_farmingService.FarmingSpots.ForEach(spot =>
			{
				try
				{
					for (var i = 0; i < spot.Objects.Count; i++)
					{
						var shape = ClShape.Get(x => x.Id == spot.Id && x.ShapeType == ColshapeType.FARMING_SPOT && x.FarmingIndex == i);
						if (shape == null) continue;
						if (shape.FarmingUsable)
						{
							if (spot.Objects[i].Health > 0) continue;

							shape.FarmingUsable = false;
							shape.FarmingDespawnTime = DateTime.Now;
							ObjectStreamer.RemoveObject(shape.FarmingObjectId);
							continue;
						}

						if (shape.FarmingDespawnTime.AddMinutes(3) >= DateTime.Now)
						{
							spot.Objects[i].Health = 100;
							shape.FarmingUsable = true;
							shape.FarmingDespawnTime = DateTime.Now;
							shape.FarmingObjectId = ObjectStreamer.AddObject(spot.ObjectHash, new Position(spot.Objects[i].Position.X, spot.Objects[i].Position.Y, spot.Objects[i].Position.Z - 1), new Rotation(), false, false, 0);
						}
					}
				}
				catch (Exception exception)
				{
					Console.WriteLine(exception.Message);
					Console.WriteLine(exception.StackTrace);
				}
			});
		}
	}
}