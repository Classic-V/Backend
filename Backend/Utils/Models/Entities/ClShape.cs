using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using Backend.Utils.Enums;
using Backend.Utils.Models.Inventory;
using Backend.Utils.Streamer;
using AltV.Net.Data;

namespace Backend.Utils.Models.Entities
{
	public class ClShape : AsyncColShape
	{
		public static List<ClShape> All { get; } = new();

		public static ClShape? Get(Func<ClShape, bool> predicate)
		{
			lock(All)
			{
				return All.FirstOrDefault(predicate);
			}
		}

		public int Id { get; set; } = 0;
		public ColshapeType ShapeType { get; set; }
		public float Size { get; set; } = 0;

		// GENERAL
		public string Interaction { get; set; } = string.Empty;
		public bool Locked { get; set; } = false;
		public InventoryModel? Inventory { get; set; } = null;
		public int InventoryOwner { get; set; } = 0;
		// 0 = player, 1 = team
		public int InventoryOwnerType { get; set; } = 0;
		public int OwnerId { get; set; } = 0;

		// JUMPPOINT
		public bool JumpPointType { get; set; } = false;

		// FARMING
		public int FarmingIndex { get; set; } = 0;
		public bool FarmingUsable { get; set; } = false;
		public DateTime FarmingDespawnTime { get; set; }
		public int FarmingObjectId { get; set; }

        // STORAGE
        public int StorageInventoryId { get; set; }

        // DEV
        public int MarkerId { get; set; }

        public ClShape(ICore core, nint nativePointer) : base(core, nativePointer)
		{
			All.Add(this);

			if (Resource.DevMode)
			{
				Task.Run(async () =>
				{
					await Task.Delay(1000);
					await CreateMarker();
				});
			}
		}

		public Task Destory()
		{
			lock (All)
			{
				if (MarkerId > 0) MarkerStreamer.RemoveMarker(MarkerId);
				All.Remove(this);
				Destroy();
			}
			return Task.CompletedTask;
		}

		private Task CreateMarker()
		{
			MarkerId = MarkerStreamer.AddMarker(new Marker(
					1,
					new Position(Position.X, Position.Y, Position.Z-1),
					new Position(Size * 2, Size * 2, 1),
					new Rgba(0, 155, 255, 255),
					Size + 50,
					Dimension));

			return Task.CompletedTask;
		}
	}
}