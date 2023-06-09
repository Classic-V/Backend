using AltV.Net.Data;
using Backend.Utils.Models.ClothesShop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player
{
	public class ClothesModel
	{
		public static readonly List<ClothesModel> MaleAdminClothes = new List<ClothesModel>()
		{
			// Spieler
			new ClothesModel(),
			
			// Guide
			new ClothesModel(),
			// Gamedesign
			new ClothesModel(),
			// Developer
			new ClothesModel(),

			// Supporter
			new ClothesModel(new(0,135,5,0),new(0,287,5,0),new(0,15,0,0),new(0,10,0,0),new(0,114,5,0),new(0,78,5,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,8,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Moderator
			new ClothesModel(new(0,135,4,0),new(0,287,4,0),new(0,15,0,0),new(0,10,0,0),new(0,114,4,0),new(0,78,4,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,8,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Administrator
			new ClothesModel(new(0,135,3,0),new(0,287,3,0),new(0,15,0,0),new(0,10,0,0),new(0,114,3,0),new(0,78,3,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,8,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Superadmin
			new ClothesModel(new(0,135,12,0),new(0,287,12,0),new(0,15,0,0),new(0,10,0,0),new(0,114,12,0),new(0,78,12,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,8,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Manager
			new ClothesModel(new(0,135,2,0),new(0,287,2,0),new(0,15,0,0),new(0,10,0,0),new(0,114,2,0),new(0,78,2,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,8,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Projektleiter
			new ClothesModel(new(0,135,2,0),new(0,287,2,0),new(0,15,0,0),new(0,10,0,0),new(0,114,2,0),new(0,78,2,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,8,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0))
		};
        public static readonly List<ClothesModel> FemaleAdminClothes = new List<ClothesModel>()
        {
			// Spieler
			new ClothesModel(),
			
			// Guide
			new ClothesModel(),
			// Gamedesign
			new ClothesModel(),
			// Developer
			new ClothesModel(),

			// Supporter
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Moderator
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Gamedesigner
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Administrator
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Entwickler
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Superadmin
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Manager
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0)),
			// Projektleiter
			new ClothesModel(new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,0,0,0),new(0,0,0,0),new(0,-1,0,0),new(0,-1,0,0))
        };

        public static readonly ClothesModel MalePrisonClothes = new ClothesModel(
            new ClothingModel(1, 0, 0, 0),
            new ClothingModel(11, 5, 0, 0),
            new ClothingModel(8, 15, 0, 0),
            new ClothingModel(3, 5, 0, 0),
            new ClothingModel(4, 5, 7, 0),
            new ClothingModel(6, 6, 0, 0),
            new ClothingModel(9, 0, 0, 0),
            new ClothingModel(5, 0, 0, 0),
            new ClothingModel(7, 0, 0, 0),
            new ClothingModel(0, 8, 0, 0),
            new ClothingModel(1, -1, 0, 0),
            new ClothingModel(2, -1, 0, 0),
            new ClothingModel(6, -1, 0, 0),
            new ClothingModel(7, -1, 0, 0)
        );

        public static readonly ClothesModel FemalePrisonClothes = new ClothesModel(
            new ClothingModel(1, 0, 0, 0),
            new ClothingModel(11, 247, 0, 0),
            new ClothingModel(8, 15, 0, 0),
            new ClothingModel(3, 11, 0, 0),
            new ClothingModel(4, 66, 7, 0),
            new ClothingModel(6, 5, 0, 0),
            new ClothingModel(9, 0, 0, 0),
            new ClothingModel(5, 0, 0, 0),
            new ClothingModel(7, 0, 0, 0),
            new ClothingModel(0, -1, 0, 0),
            new ClothingModel(1, -1, 0, 0),
            new ClothingModel(2, -1, 0, 0),
            new ClothingModel(6, -1, 0, 0),
            new ClothingModel(7, -1, 0, 0)
        );

        public ClothingModel Mask { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Top { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Undershirt { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Body { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Leg { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Shoe { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Armor { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Bag { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Accessories { get; set; } = new(0, 0, 0, 0);
		public ClothingModel Decals { get; set; } = new(0, 0, 0, 0);

		// PROPS
		public ClothingModel Hat { get; set; } = new(0, -1, 0, 0);
		public ClothingModel Glasses { get; set; } = new(0, -1, 0, 0);
		public ClothingModel Ears { get; set; } = new(0, -1, 0, 0);
		public ClothingModel Watch { get; set; } = new(0, -1, 0, 0);
		public ClothingModel Bracelet { get; set; } = new(0, -1, 0, 0);

		public ClothesModel()
		{
		}

		public ClothesModel(ClothingModel mask, ClothingModel top, ClothingModel undershirt, ClothingModel body, ClothingModel leg, ClothingModel shoe, ClothingModel armor, ClothingModel bag, ClothingModel accessories, ClothingModel hat, ClothingModel glasses, ClothingModel ears, ClothingModel watch, ClothingModel bracelet)
		{
			Mask = mask;
			Top = top;
			Undershirt = undershirt;
			Body = body;
			Leg = leg;
			Shoe = shoe;
			Armor = armor;
			Bag = bag;
			Accessories = accessories;
			Hat = hat;
			Glasses = glasses;
			Ears = ears;
			Watch = watch;
			Bracelet = bracelet;
		}
	}
}