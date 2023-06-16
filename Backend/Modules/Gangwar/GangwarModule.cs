using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Gangwar.Interface;
using Backend.Data;
using Backend.Services.Team.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Gangwar;
using Backend.Utils.Models.Player.Client;
using Newtonsoft.Json;

namespace Backend.Modules.Gangwar
{
    public class GangwarModule : Module<GangwarModule>, IEventColshape, IEventIntervalThirtySecond
    {
        private readonly List<(WeaponModel Hash, string Name)> _weapons = new()
        {
            (WeaponModel.AssaultRifle, "Assaultrifle"),
			(WeaponModel.CarbineRifle, "Carbinerifle"),
			(WeaponModel.AdvancedRifle, "Advancedrifle"),
			(WeaponModel.BullpupRifle, "Bullpuprifle"),

			(WeaponModel.SpecialCarbine, "Specialcarbine"),
			(WeaponModel.GusenbergSweeper, "Gusenberg"),
			(WeaponModel.AssaultRifleMkII, "Assaultrifle MKII"),
		};

		private readonly IGangwarController _gangwarController;
        private readonly ITeamService _teamService;

        public GangwarModule(IGangwarController gangwarController, ITeamService teamService, IEventController eventController) : base("Gangwar")
        {
            _gangwarController = gangwarController;
            _teamService = teamService;

            eventController.OnClient<int>("Server:Gangwar:Start", StartGangwar);
            eventController.OnClient("Server:Gangwar:Join", JoinGangwar);
            eventController.OnClient("Server:Gangwar:SpawnVehicle", SpawnGangwarVehicle);
            eventController.OnClient("Server:Gangwar:Interact", OpenGangwarMenu);
			eventController.OnClient<int>("Server:Gangwar:SelectKit", SelectKit);
			eventController.OnClient("Server:Gangwar:OpenMenu", OpenMenu);
			eventController.OnClient("Server:Gangwar:PlayerQuit", QuitGangwar);
		}

		private async void QuitGangwar(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || !player.IsInGangwar) return;

            await _gangwarController.PlayerQuitGangwar(player);
		}

		private void OpenMenu(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null || !player.IsInGangwar || player.DbModel.Team != player.CurrentShape?.OwnerId) return;

            player.ShowNativeMenu(true, new ClientNativeMenu("Gangwar", new List<ClientNativeMenuItem>()
            {
	            new ClientNativeMenuItem("Fahrzeug Spawnen") { Close = true, CallbackEvent = "Server:Gangwar:SpawnVehicle" }, 
	            new ClientNativeMenuItem("Gangwar verlassen") { Close = true, CallbackEvent = "Server:Gangwar:PlayerQuit" }
			}));
		}

		private void SelectKit(ClPlayer player, string eventKey, int index)
        {
            if (player.DbModel == null || !player.IsInGangwar) return;

            var team = _teamService.GetTeam(player.DbModel.Team).Result;
            if(team == null) return;

            player.GangwarWeapon = index;
            player.RemoveAllWeapons();

            player.Emit("Client:PlayerModule:SetWeapons", JsonConvert.SerializeObject(new List<ClientWeaponModel>()
            {
                new ClientWeaponModel((uint)_weapons[index].Hash, 9999, new List<uint>()),
		    	new ClientWeaponModel((uint)WeaponModel.PistolMkII, 9999, new List<uint>()),
				new ClientWeaponModel(team.MeeleWeaponHash, 0, new List<uint>())
			}));

            player.GiveWeapon((uint)_weapons[index].Hash, 9999, false);
			player.GiveWeapon((uint)WeaponModel.PistolMkII, 9999, false);
			player.GiveWeapon(team.MeeleWeaponHash, 0, false);
            player.ShowComponent("GangwarWeapon", false);
		}

		private void OpenGangwarMenu(ClPlayer player, string eventKey)
        {
            if (player.CurrentShape == null || player.CurrentShape.ShapeType != ColshapeType.GANGWAR_START) return;

            player.ShowNativeMenu(true, new ClientNativeMenu("Gangwar Gebiet", new List<ClientNativeMenuItem>()
            {
	            new ClientNativeMenuItem("Angreifen") { Close = true, CallbackEvent = "Server:Gangwar:Start", CallbackArgs = new object[] {player.CurrentShape.Id}}
            }));
        }

        private async void SpawnGangwarVehicle(ClPlayer player, string eventKey)
        {
            if (!player.IsInGangwar || player.CurrentShape == null) return;

            var runningGangwar = await _gangwarController.GetGangwarModel(player.CurrentShape.Id);
            if (runningGangwar == null)
            {
                await player.Notify($"GANGWAR", "Fehler beim Abfragen der Gangwar-Daten.", NotificationType.ERROR);
                return;
            }

            var team = await _teamService.GetTeam(player.DbModel.Team);
            if (team == null)
            {
                await player.Notify($"TEAM", "Fehler beim Abfragen der Team-Daten", NotificationType.ERROR);
                return;
            }

            var parkoutPositionList = player.DbModel.Team == runningGangwar.AttackerId ? runningGangwar.AttackerVehicleSpawnPosition : runningGangwar.DefenderVehicleSpawnPosition;
            var nextFreeParkoutPosition = await GetFreePosition(parkoutPositionList);
            if (nextFreeParkoutPosition == null!)
            {
                await player.Notify($"GANGWAR", "Es sind keine Parkplätze mehr frei.", NotificationType.ERROR);
                return;
            }

			var vehicle = new ClVehicle(Alt.Core, 0xCE44C4B9, nextFreeParkoutPosition.Position, new Rotation(0, 0, nextFreeParkoutPosition.H), null, 100);
            vehicle.GangwarId = runningGangwar.Id;
            await vehicle.SetLockState(false);
			await vehicle.SetEngineState(true);

            vehicle.Dimension = runningGangwar.Id;
            vehicle.NumberplateText = team.ShortName;
            vehicle.PrimaryColorRgb = team.Color.Rgba;
            vehicle.SecondaryColorRgb = team.Color.Rgba;
        }

        private async void JoinGangwar(ClPlayer player, string eventKey)
        {
            var runningGangwar = await _gangwarController.GetRunningGangwarByTeam(player.DbModel.Team);
            if (runningGangwar == null)
            {
                await player.Notify("GANGWAR", "Dein Team greift kein Gangwar-Gebiet an!", NotificationType.ERROR);
                return;
            }

            if (await _gangwarController.IsGangwarFull(runningGangwar.Id, player.DbModel.Team))
            {
                await player.Notify("GANGWAR", "Der Gangwar ist bereits voll!", NotificationType.ERROR);
                return;
            }

            var team = await _teamService.GetTeam(player.DbModel.Team);
            if (team == null) return;

            await player.ShowComponent("Team", false);

            player.IsInGangwar = true;
            var spawnPosition = player.DbModel.Team == runningGangwar.AttackerId ? runningGangwar.AttackerSpawnPosition : runningGangwar.DefenderSpawnPosition;
            await player.SetDimension(runningGangwar.Id);
            player.IsInInterior = true;
            player.OutsideInteriorPosition = player.Position;
            await player.SetPosition(spawnPosition.Position);
            player.Rotation = new Rotation(0, 0, spawnPosition.H);

            if(player.GangwarWeapon == -1)
            {
				var specialWeapon = new Random().Next(0, 4);
				var weapons = new List<GangwarWeaponModel>();

				for (var i = 0; i < _weapons.Count - 3; i++)
					weapons.Add(new GangwarWeaponModel(i, _weapons[i].Name, "Pistol MKII", team.MeeleWeapon));

				if (specialWeapon > 0)
					weapons.Add(new GangwarWeaponModel(specialWeapon + 3, _weapons[specialWeapon+3].Name, "Pistol MKII", team.MeeleWeapon));

				await player.ShowComponent("GangwarWeapon", true, JsonConvert.SerializeObject(weapons));
			}
            else
            {
                SelectKit(player, "", player.GangwarWeapon);
            }

            await _gangwarController.OnJoin(player, runningGangwar.Id);
		}

		private Task<PositionModel?> GetFreePosition(List<PositionModel> positions)
        {
            return Task.FromResult(positions.FirstOrDefault(x =>
            {
                bool free = true;
                for (var i = 0; i < ClVehicle.All.Count; i++)
                {
                    if (ClVehicle.All[i].Position.Distance(x.Position) < 5)
                    {
                        free = false;
                        break;
                    }
                }

                return free;
            }));
        }

        private async void StartGangwar(ClPlayer player, string eventKey, int gangwarId)
        {
            if (!await _gangwarController.AttackGangwar(player, gangwarId, player.DbModel.Team)) return;

            JoinGangwar(player, "");
        }

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player) return Task.CompletedTask;

			var player = (ClPlayer)entity;

            if (player.DbModel.Team < 4) return Task.CompletedTask;

			switch (shape.ShapeType)
            {
                case ColshapeType.GANGWAR_START:
					player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_GANGWAR : Interactions.NONE);
					break;
                case ColshapeType.GANGWAR_SPAWN:
					player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_GANGWARSPAWN : Interactions.NONE);
					break;
			}
			
			return Task.CompletedTask;
		}

		public async Task OnEveryThirtySeconds()
		{
            await _gangwarController.FlagTick();
		}
	}
}
