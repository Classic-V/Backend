using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Medic;

namespace Backend.Modules.Medic
{
	public class MedicModule : Module<MedicModule>, IEventColshape
	{
		private readonly List<HospitalModel> Hospitals = new()
		{
			// Sandy
			new(
				new(1826.7957f, 3693.6396f, 34.216797f),
				new()
				{
					new(1819.6879f, 3673.055f, 35.19409f, 0f),
					new(1826.3605f, 3676.800f, 35.19409f, 0f),
					new(1821.9692f, 3680.5715f, 34.975098f, 0f),
					new(1818.7517f, 3678.5276f, 35.00879f, 0f),
					new(1831.1736f, 3674.0571f, 34.975098f, 0f),
					new(1828.4967f, 3678.3691f, 34.975098f, 0f),
				})
		};

		public MedicModule() : base("Medic")
		{
			Hospitals.ForEach(x =>
			{
				var index = Hospitals.IndexOf(x);
				var shape = (ClShape)Alt.CreateColShapeSphere(x.EntryPosition, 3f);
				shape.Id = index;
				shape.ShapeType = ColshapeType.MEDIC_INPUT;
				shape.Size = 3f;
			});
		}

		public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.MEDIC_INPUT || !entered) return;

			var player = (ClPlayer)entity;
			if (player.DbModel.Team != 3 || !player.DbModel.Alive || !player.IsInVehicle) return;

			var target = ClPlayer.All.FirstOrDefault(x => x.Vehicle == player.Vehicle && !x.DbModel.Alive);
			if (target == null) return;

			var pos = Hospitals[shape.Id].BedPositions.FirstOrDefault(x => ClPlayer.All.FirstOrDefault(e => e.Position.Distance(x.Position) < 1f) == null);
			if(pos == null)
			{
				await player.Notify("Krankenhaus", "Kein Krankenbett ist verfügbar für eine Einlieferung!", NotificationType.ERROR);
				return;
			}

			await target.SetPosition(pos.Position);
			target.Rotation = new(0, 0, pos.H);
			target.InHostpital = true;
		}
	}
}