using Backend.Controllers.Event.Interface;
using Backend.Controllers.Vehicle.Interface;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Enums;

namespace Backend.Modules.Vehicle
{
	public class VehicleModule : Module<VehicleModule>
	{
		private readonly IVehicleController _vehicleController;

		public VehicleModule(IEventController eventController, IVehicleController vehicleController) : base("Vehicle")
		{
			_vehicleController = vehicleController;

			eventController.OnClient<int>("Server:Vehicle:Eject", EjectPlayer);
			eventController.OnClient<float>("Server:Vehicle:UpdateFuel", UpdateVehicleFuel);
			eventController.OnClient("Server:Vehicle:Lock", LockVehicle);
			eventController.OnClient("Server:Vehicle:LockTrunk", LockVehicleTrunk);
			eventController.OnClient("Server:Vehicle:ToggleSirenState", SetSirenState);
		}

		private void EjectPlayer(ClPlayer player, string eventKey, int seat)
		{
			if (player.DbModel == null || !player.IsInVehicle || player.Seat != 1) return;

			if(seat == 1)
			{
				player.Notify("Fahrzeug", "Du kannst dich nicht selbst rauswerfen!", NotificationType.ERROR);
				return;
			}

			var target = ClPlayer.All.FirstOrDefault(x => x.Vehicle == player.Vehicle && x.Seat == seat);
			if (target == null) return;

			target.Emit("Client:Vehicle:Exit");
			target.Notify("Fahrzeug", "Du wurdest aus dem Fahrzeug geworfen!", NotificationType.INFO);
		}

		private void UpdateVehicleFuel(ClPlayer player, string eventKey, float mod)
		{
			if (player.DbModel == null || !player.IsInVehicle) return;

			var veh = (ClVehicle)player.Vehicle;
			if (veh.DbModel == null) return;

			veh.SetFuel(veh.DbModel.Fuel - mod);
		}

		private void SetSirenState(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || !player.IsInVehicle) return;

			var veh = (ClVehicle)player.Vehicle;
			veh.SetSirenSoundActive(!veh.SirenSoundActive);
		}

		private async void LockVehicle(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null) return;

			var vehicle = player.IsInVehicle ? (ClVehicle)player.Vehicle : GetClosestVehicle(player, 7f);
			if (vehicle == null || !IsVehicleOwner(vehicle, player)) return;

            await vehicle.SetLockState(!vehicle.Locked);
			if (vehicle.Locked) await player.Notify("FAHRZEUG", $"Fahrzeug abgeschlossen.", NotificationType.ERROR);
			else await player.Notify("FAHRZEUG", $"Fahrzeug aufgeschlossen.", NotificationType.SUCCESS);
		}

		private async void LockVehicleTrunk(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null) return;

			var vehicle = player.IsInVehicle ? (ClVehicle)player.Vehicle : GetClosestVehicle(player, 7f);
			if (vehicle == null || vehicle.DbModel == null || vehicle.Locked) return;

			vehicle.TrunkLocked = !vehicle.TrunkLocked;
			vehicle.SetDoorState(5, (byte)(vehicle.TrunkLocked ? 0 : 7));
			if(vehicle.TrunkLocked) await player.Notify("FAHRZEUG", $"Kofferraum abgeschlossen.", NotificationType.ERROR);
			else await player.Notify("FAHRZEUG", $"Kofferraum aufgeschlossen.", NotificationType.SUCCESS);
		}

		private ClVehicle? GetClosestVehicle(ClPlayer player, float range)
		{
			var vehicles = ClVehicle.All.Where(x => x.Position.Distance(player.Position) <= range).ToList();
			ClVehicle? vehicle = null;
			var dist = range;

			for(var i = 0; i < vehicles.Count; i++)
				if (player.Position.Distance(vehicles[i].Position) < dist) vehicle = vehicles[i];

			return vehicle;
		}

		private bool IsVehicleOwner(ClVehicle vehicle, ClPlayer player)
		{
			return vehicle.DbModel == null || (vehicle.DbModel.Type == VehicleType.TEAM && vehicle.DbModel.Owner == player.DbModel.Team) || (vehicle.DbModel.Type == VehicleType.PLAYER && vehicle.DbModel.Owner == player.DbModel.Id) || (vehicle.DbModel.Type == VehicleType.SWAT && player.DbModel.Team > 0 && player.DbModel.Team < 3 && player.DbModel.TeamStoragePermission) || (vehicle.DbModel.Type == VehicleType.TEMPORARY && vehicle.DbModel.Owner == player.DbModel.Id);
		}
	}
}