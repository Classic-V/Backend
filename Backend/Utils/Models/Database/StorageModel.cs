using AltV.Net.Data;
using Backend.Utils.Enums;
using Backend.Utils.Models.Inventory;

namespace Backend.Utils.Models.Database
{
    public class StorageModel
	{
		public static Position SmallUpgradePosition { get; } = new(1088.3605f, -3101.3274f, -39.01245f);
		public static Position SmallExitPosition { get; } = new(1087.134f, -3099.389f, -39.01245f);
		public static List<PositionModel> SmallPositions { get; } = new()
        {
            new(1088.7164f, -3096.7780f, -39.01245f, 0f),
            new(1091.3011f, -3096.7780f, -39.01245f, 0f),
            new(1095.0198f, -3096.7780f, -39.01245f, 0f),
			new(1097.6307f, -3096.7780f, -39.01245f, 0f),
			new(1101.2836f, -3096.7780f, -39.01245f, 0f),
			new(1103.8682f, -3096.7780f, -39.01245f, 0f)
		};

		public static Position MediumExitPosition { get; } = new(1073.0637f, -3102.5144f, -39.01245f);
		public static Position MediumUpgradePosition { get; } = new(1049.0374f, -3100.6946f, -39.01245f);
		public static List<PositionModel> MediumPositions { get; } = new()
		{
			new(1067.6307f, -3109.5298f, -39.01245f, 0f),
			new(1065.2043f, -3109.5298f, -39.01245f, 0f),
			new(1062.8044f, -3109.5298f, -39.01245f, 0f),
			new(1060.3649f, -3109.5298f, -39.01245f, 0f),
			new(1057.9385f, -3109.5298f, -39.01245f, 0f),
			new(1055.5516f, -3109.5298f, -39.01245f, 0f),
			new(1053.1516f, -3109.5298f, -39.01245f, 0f),

			new(1053.0593f, -3102.6462f, -39.01245f, 0f),
			new(1055.4330f, -3102.6462f, -39.01245f, 0f),
			new(1057.8989f, -3102.6462f, -39.01245f, 0f),
			new(1060.1274f, -3102.6462f, -39.01245f, 0f),
			new(1062.6329f, -3102.6462f, -39.01245f, 0f),
			new(1064.9670f, -3102.6462f, -39.01245f, 0f),
			new(1067.4462f, -3102.6462f, -39.01245f, 0f),

			new(1067.4594f, -3095.7363f, -39.01245f, 0f),
			new(1065.1120f, -3095.7363f, -39.01245f, 0f),
			new(1062.6989f, -3095.7363f, -39.01245f, 0f),
			new(1060.2594f, -3095.7363f, -39.01245f, 0f),
			new(1057.8857f, -3095.7363f, -39.01245f, 0f),
			new(1055.3802f, -3095.7363f, -39.01245f, 0f),
			new(1053.0066f, -3095.7363f, -39.01245f, 0f)
		};

		public static Position HighExitPosition{ get; } = new(1027.6879f, -3101.3933f, -39.01245f);
		public static Position HighUpgradePosition { get; } = new(994.5099f, -3100.233f, -39.01245f);
		public static List<PositionModel> HighPositions { get; } = new()
		{
			new(1026.4099f, -3106.5364f, -39.01245f, 0f),
			new (1026.4099f, -3108.9758f, -39.01245f, -1.5336909f),
			new(1026.4099f, -3111.1912f, -39.01245f, 1.5336909f),

			new(1018.18024f, -3108.5276f, -39.01245f, 1.5336909f),
			new(1015.7934f, -3108.5276f, -39.01245f, 0f),
			new(1013.24835f, -3108.5276f, -39.01245f, 0f),
			new(1010.96704f, -3108.5276f, -39.01245f, 1.5336909f),
			new(1008.60657f, -3108.5276f, -39.01245f, 0f),
			new(1006.03516f, -3108.5276f, -39.01245f, 0f),
			new(1003.45056f, -3108.5276f, -39.01245f, 1.5336909f),

			new(993.3143f, -3111.3098f, -39.01245f, -1.5336909f),
			new(993.3143f, -3109.0022f, -39.01245f, 1.5336909f),
			new(993.3143f, -3106.5364f, -39.01245f, 0f),

			new(1003.54285f, -3102.923f, -39.01245f, 0f),
			new(1005.87695f, -3102.923f, -39.01245f, 0f),
			new(1008.30330f, -3102.923f, -39.01245f, 1.5336909f),
			new(1010.91430f, -3102.923f, -39.01245f, 0f),
			new(1013.32745f, -3102.923f, -39.01245f, 0f),
			new(1015.81976f, -3102.923f, -39.01245f, 1.5336909f),
			new(1017.99560f, -3102.923f, -39.01245f, 0f),

			new(1018.12744f, -3097.0022f, -39.01245f, 0f),
			new(1015.55600f, -3097.0022f, -39.01245f, 1.5336909f),
			new(1013.14290f, -3097.0022f, -39.01245f, 0f),
			new(1010.82196f, -3097.0022f, -39.01245f, 0f),
			new(1008.63300f, -3097.0022f, -39.01245f, 1.5336909f),
			new(1005.99560f, -3097.0022f, -39.01245f, 0f),
			new(1003.84610f, -3097.0022f, -39.01245f, 0f),

			new(1003.56920f, -3092.0308f, -39.01245f, 1.5336909f),
			new(1005.75824f, -3092.0308f, -39.01245f, 0f),
			new(1008.25055f, -3092.0308f, -39.01245f, 0f),
			new(1010.75604f, -3092.0308f, -39.01245f, 1.5336909f),
			new(1013.24835f, -3092.0308f, -39.01245f, 0f),
			new(1015.52966f, -3092.0308f, -39.01245f, 0f),
			new(1017.99560f, -3092.0308f, -39.01245f, 1.5336909f),

			new(1026.4099f, -3091.6484f, -39.01245f, 1.5336909f),
			new(1026.4099f, -3093.8638f, -39.01245f, -1.5336909f),
			new(1026.4099f, -3096.29f, -39.01245f, 0f)
		};

		public int Id { get; set; }
        public int OwnerId { get; set; }
        public Position Position { get; set; }
        public WarehouseType WarehouseType { get; set; }
        public List<InventoryModel> Inventories { get; set; } = new ();

        public StorageModel() { }

        public StorageModel(int owner, Position pos, WarehouseType type, List<InventoryModel> inventories)
        {
            OwnerId = owner;
            Position = pos;
            WarehouseType = type;
            Inventories = inventories;
        }

        public int MaxInventorySlots()
        {
            return (WarehouseType == WarehouseType.SMALL_WAREHOUSE) ? 6 : (WarehouseType == WarehouseType.MEDIUM_WAREHOUSE) ? 21 : 37;
        }
    }
}
