using AltV.Net;
using Backend.Controllers.Drop.Interface;
using Backend.Data;
using Backend.Utils.Models.Database;
using Backend.Utils;
using Backend.Utils.Enums;
using Backend.Utils.Streamer;
using Backend.Utils.Models.Inventory;
using Backend.Modules.Inventory;
using AltV.Net.Data;
using Backend.Utils.Models.Entities;
using AltV.Net.Elements.Entities;
using AltV.Net.Async;
using AltV.Net.Enums;
using Backend.Services.Drop.Interface;

namespace Backend.Controllers.Drop
{
	public class DropController : IDropController
	{
		public bool IsDropOpen { get; private set; }

		private static readonly List<int> Items = new()
		{
			// ITEMS
			3,
			// PISTOL
			130,
			138,
			139,
			// RIFLE
			150,
			151,
			152,
			153,
			154,
			155,
			156,
			157,
			158,
			159,
			160,
			161,
			162,
			// SNIPER
			170,
			171,
			172,
			174
		};

		private static readonly Random Random = new Random();

		private readonly IDropService _dropService;

		private DropModel? ActiveDrop { get; set; }
		private DateTime Created { get; set; } = DateTime.Now;
		private IBlip? Blip { get; set; }
		private ClShape? MainShape { get; set; }
		private ClShape? LootShape1 { get; set; }
		private ClShape? LootShape2 { get; set; }
		private ClShape? LootShape3 { get; set; }
		private List<int> Objects = new();

		public DropController(IDropService dropService)
		{
			_dropService = dropService;
			IsDropOpen = false;
		}

		public async Task CreateDrop()
		{
			if (ActiveDrop != null) return;

			ActiveDrop = _dropService.Drops[Random.Next(_dropService.Drops.Count)];
			//var type = Convert.ToBoolean(Random.Next(2));
			var type = false;
			var objPos = ActiveDrop.Position.Position - (type ? new Position(0, 0, 1.5f) : new Position(15, 9, 46));
			Created = DateTime.Now;

			MainShape = (ClShape)Alt.CreateColShapeSphere(ActiveDrop.Position.Position, 15);
			MainShape.ShapeType = ColshapeType.DROP_MAIN;
			MainShape.Size = 15;
			Objects.Add(ObjectStreamer.AddObject((uint)(type ? 538442956 : 249853152), objPos, new Rotation(0, 0, ActiveDrop.Position.H), false, false, 0));

			for (var i = 0; i < ClPlayer.All.Count; i++)
				await ClPlayer.All[i].ShowGlobalNotify("Militärischer Absturz", "Es wurde ein Militärischer Flugzeug Absturz gemeldet.", 1000 * 10);
		}

		public Task OpenDrop()
		{
			if (ActiveDrop == null || IsDropOpen) return Task.CompletedTask;

			var down = new Position(0, 0, -1);

			LootShape1 = (ClShape)Alt.CreateColShapeSphere(ActiveDrop.LootCrate1.Position, 5);
			LootShape1.Id = 1;
			LootShape1.ShapeType = ColshapeType.DROP_BOX_1;
			LootShape1.Size = 5;
			LootShape1.Inventory = GetRandomLoot(InventoryType.DROP_BOX_1, 8);
			Objects.Add(ObjectStreamer.AddObject(1776043012, ActiveDrop.LootCrate1.Position + down, new Rotation(0, 0, ActiveDrop.LootCrate1.H), false, false, 0));

			LootShape2 = (ClShape)Alt.CreateColShapeSphere(ActiveDrop.LootCrate2.Position, 5);
			LootShape2.Id = 2;
			LootShape2.ShapeType = ColshapeType.DROP_BOX_2;
			LootShape2.Size = 5;
			LootShape2.Inventory = GetRandomLoot(InventoryType.DROP_BOX_2, 4);
			Objects.Add(ObjectStreamer.AddObject(3593631139, ActiveDrop.LootCrate2.Position + down, new Rotation(0, 0, ActiveDrop.LootCrate2.H), false, false, 0));

			LootShape3 = (ClShape)Alt.CreateColShapeSphere(ActiveDrop.LootCrate3.Position, 5);
			LootShape3.Id = 3;
			LootShape3.ShapeType = ColshapeType.DROP_BOX_3;
			LootShape3.Size = 5;
			LootShape3.Inventory = GetRandomLoot(InventoryType.DROP_BOX_3, 4);
			Objects.Add(ObjectStreamer.AddObject(3593631139, ActiveDrop.LootCrate3.Position + down, new Rotation(0, 0, ActiveDrop.LootCrate3.H), false, false, 0));

			IsDropOpen = true;

			return Task.CompletedTask;
		}

		public async Task Tick()
		{
			if(ActiveDrop == null)
			{
				if (Created.AddHours(2) > DateTime.Now || new Random().Next(100) < 98) return;

				await CreateDrop();

				return;
			}

			if(Created.AddMinutes(30) > DateTime.Now || ClPlayer.All.FirstOrDefault(x => x.Position.Distance(ActiveDrop.Position.Position) < 30) != null) return;

			await RemoveDrop();

			return;
		}

		public Task RemoveDrop()
		{
			if (ActiveDrop == null) return Task.CompletedTask;

			MainShape?.Remove();
			LootShape1?.Remove();
			LootShape2?.Remove();
			LootShape3?.Remove();
			Blip?.Remove();
			ActiveDrop = null;
			for(var i = 0; i < Objects.Count; i++)
				ObjectStreamer.RemoveObject(Objects[i]);
			IsDropOpen = false;

			return Task.CompletedTask;
		}

		public InventoryModel GetRandomLoot(InventoryType type, int slots)
		{
			var inventory = new InventoryModel(1000f, slots, type);

			for(var i = 0; i < slots; i++)
			{
				var item = Items[Random.Next(Items.Count)];
				var itemBase = InventoryModule.ItemModels.FirstOrDefault(x => x.Id == item);
				if(itemBase != null) inventory.AddItem(itemBase, itemBase.MaxAmount);
			}

			return inventory;
		}
	}
}