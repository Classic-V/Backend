using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Gangwar.Interface;
using Backend.Services.Gangwar.Interface;
using Backend.Services.Team.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;
using Backend.Utils.Models;
using AltV.Net.Data;
using Newtonsoft.Json;
using Backend.Utils.Models.Gangwar.Client;
using System;
using Backend.Modules.Inventory;
using AltV.Net.Enums;
using Backend.Utils.Models.Gangwar;

namespace Backend.Controllers.Gangwar
{
    public class GangwarController : IGangwarController
    {
        private readonly IGangwarService _gangwarService;
        private readonly ITeamService _teamService;

        public GangwarController(IGangwarService gangwarService, ITeamService teamService)
        {
            _gangwarService = gangwarService;
            _teamService = teamService;

            LoadGangwars();
        }

        private async void LoadGangwars()
        {
            await Task.Delay(5000);

			_gangwarService.Gangwar.ForEach(LoadGangwar);
		}

        private async void LoadGangwar(GangwarModel model)
		{
            var team = await _teamService.GetTeam(model.OwnerId);

            var blip = Alt.CreateBlip((byte)BlipType.Destination, model.AttackPosition);
            blip.Name = $"Gangwar Gebiet";
            blip.Sprite = 543;
            blip.ShortRange = true;
            blip.Color = (byte)(team == null ? 4 : team.BlipColor);

            var startColshape = (ClShape)Alt.CreateColShapeSphere(model.AttackPosition, 1f);
            startColshape.Id = model.Id;
            startColshape.ShapeType = ColshapeType.GANGWAR_START;
            startColshape.Size = 1f;

            MarkerStreamer.AddMarker(new Marker(1, model.AttackPosition - new Position(0, 0, 1), new Position(2f, 2f, 1f), new Rgba(0, 155, 255, 255), 100, 0));
        }

        public async Task<bool> AttackGangwar(ClPlayer player, int id, int attackerId)
        {
            var gangwarModel = _gangwarService.GetGangwar(id);
            if (gangwarModel == null) return false;

            var now = DateTime.Now;
            var attackable = gangwarModel.LastAttacked.AddHours(36);

            if(gangwarModel.OwnerId == 0)
            {
                gangwarModel.OwnerId = player.DbModel.Team;
                gangwarModel.LastAttacked = DateTime.Now;
                await player.Notify("Gangwar", "Du hast das Gebiet für deine Fraktion beansprucht!", NotificationType.SUCCESS);
                return false;
            }

            //if(now.Hour < 18)
		//	{
		//		await player.Notify("Gangwar", "Das Gangwar Gebiet kann erst ab 18 Uhr angegriffen werden!", NotificationType.ERROR);
		//		return false;
            //}

			if (attackable > now)
			{
				await player.Notify("GANGWAR", $"Das Gangwar Gebiet wurde bereits angegriffen! (Verfügbar in {Math.Floor((attackable - DateTime.Now).TotalHours)} Stunden)", NotificationType.ERROR);
				return false;
			}

			var ownerTeam = await _teamService.GetTeam(gangwarModel.OwnerId);
            if (ownerTeam == null) return false;

			var attackerTeam = await _teamService.GetTeam(attackerId);
            if (attackerTeam == null) return false;

			List<ClPlayer> ownerTeamList = ClPlayer.All.Where(x => x.DbModel.Team == ownerTeam.Id).ToList();
            List<ClPlayer> attackerTeamList = ClPlayer.All.Where(x => x.DbModel.Team == attackerTeam.Id).ToList();

			//if (ownerTeamList.Count < 10 || attackerTeamList.Count < 10)
			//{
			//	await player.Notify("", "");
			//	return;
			//}

			ownerTeamList.ForEach(async player =>
            {
                await player.Notify("GANGWAR", $"Euer Gangwar Gebiet {gangwarModel.Name} wird angegriffen!", NotificationType.INFO, 5000);
            });

            attackerTeamList.ForEach(async player =>
            {
                await player.Notify("GANGWAR", $"Ihr greift das Gangwar Gebiet {gangwarModel.Name} an!", NotificationType.INFO, 5000);
            });

            gangwarModel.LastAttacked = DateTime.Now;
            gangwarModel.AttackerId = attackerTeam.Id;
            gangwarModel.IsRunning = true;

			await StartGangwar(gangwarModel);
            return true;
        }

        public async Task EndGangwar(int id)
        {
            var gangwarModel = _gangwarService.GetGangwar(id);
            if (gangwarModel == null) return;

			var attacker = (await _teamService.GetTeam(gangwarModel.AttackerId))!;
			var defender = (await _teamService.GetTeam(gangwarModel.OwnerId))!;

			var players = new List<LeaderboardModel>();
			for (var i = 0; i < (gangwarModel.Leaderboard.Count < 5 ? gangwarModel.Leaderboard.Count : 5); i++)
			{
				var best = gangwarModel.Leaderboard.MaxBy(x => x.Kills);
				if (best == null) continue;

				players.Add(best);
				gangwarModel.Leaderboard.Remove(best);
			}

			var data = new GangwarLeaderboardModel(attacker.Name, gangwarModel.AttackerPoints, defender.Name, gangwarModel.DefenderPoints, players);

			foreach (var player in ClPlayer.All.Where(x => x.IsInGangwar && x.Dimension == id))
            {
				await PlayerQuitGangwar(player);

                await player.ShowComponent("GangwarStats", true, JsonConvert.SerializeObject(data));
			}

            gangwarModel.IsRunning = false;
			MarkerStreamer.RemoveMarker(gangwarModel.MarkerId);
            gangwarModel.ColShape?.Destroy();
            gangwarModel.AttackerShape?.Destroy();
            gangwarModel.DefenderShape?.Destroy();

			gangwarModel.Flags.ForEach(item =>
            {
                MarkerStreamer.RemoveMarker(item.FlagMarkerId);
                item.FlagShape.Destroy();
            });

			foreach (var vehicle in ClVehicle.All.Where(x => x.GangwarId == id))
                vehicle.Destroy();

			if (gangwarModel.AttackerPoints > gangwarModel.DefenderPoints)
            {
                gangwarModel.OwnerId = gangwarModel.AttackerId;
			}

			gangwarModel.AttackerPoints = 0;
            gangwarModel.DefenderPoints = 0;
            gangwarModel.AttackerId = 0;
        }

        public async Task PlayerQuitGangwar(ClPlayer player)
        {
            var team = await _teamService.GetTeam(player.DbModel.Team);
            if (team == null) return;

            player.IsInGangwar = false;
            await player.EmitBrowser("Hud:ShowGangwar", false, "");
			player.RemoveAllWeapons();
			player.IsInInterior = false;
			await player.SetPosition(team.Position);
            await player.SetDimension(0);
            await player.ApplyWeapons();
        }

        private Task StartGangwar(GangwarModel model)
        {
            model.ColShape = (ClShape)Alt.CreateColShapeSphere(model.AttackPosition, model.Radius);
            model.ColShape.Id = model.Id;
            model.ColShape.ShapeType = ColshapeType.GANGWAR;
            model.ColShape.Size = model.Radius;
            model.ColShape.Dimension = model.Id;

            model.AttackerShape = (ClShape)Alt.CreateColShapeSphere(model.AttackerSpawnPosition.Position, 2f);
			model.AttackerShape.Id = model.Id;
			model.AttackerShape.ShapeType = ColshapeType.GANGWAR_SPAWN;
			model.AttackerShape.Size = 2f;
			model.AttackerShape.Dimension = model.Id;
			model.AttackerShape.OwnerId = model.AttackerId;

            model.AttackerMarker = MarkerStreamer.AddMarker(new Marker(1, model.AttackerSpawnPosition.Position - new Position(0, 0, 1), new Position(2f, 2f, 1f), new Rgba(0, 155, 255, 255), 1000, model.Id));

			model.DefenderShape = (ClShape)Alt.CreateColShapeSphere(model.DefenderSpawnPosition.Position, 2f);
			model.DefenderShape.Id = model.Id;
			model.DefenderShape.ShapeType = ColshapeType.GANGWAR_SPAWN;
			model.DefenderShape.Size = 2f;
			model.DefenderShape.Dimension = model.Id;
            model.DefenderShape.OwnerId = model.OwnerId;

			model.DefenderMarker = MarkerStreamer.AddMarker(new Marker(1, model.DefenderSpawnPosition.Position - new Position(0, 0, 1), new Position(2f, 2f, 1f), new Rgba(0, 155, 255, 255), 1000, model.Id));

			model.MarkerId = MarkerStreamer.AddMarker(new Marker(
                1,
                model.AttackPosition - new Position(0, 0, 50),
                new Position(model.Radius*2, model.Radius*2, 100),
                new Rgba(0, 155, 255, 255),
                1000,
                model.Id));

            model.Flags.ForEach(item =>
            {
                var index = model.Flags.IndexOf(item);

                item.FlagShape = (ClShape)Alt.CreateColShapeSphere(item.FlagPosition, 1.5f);
                item.FlagShape.Id = index+1;
                item.FlagShape.ShapeType = ColshapeType.GANGWAR_FLAG;
                item.FlagShape.Size = 1.5f;
                item.FlagShape.Dimension = model.Id;

				item.FlagMarkerId = MarkerStreamer.AddMarker(new Marker(
				    4,
				    item.FlagPosition,
                    new Position(1, 1, 1),
				    new Rgba(0, 155, 255, 255),
				    1000,
                    false,
                    true,
                    false,
				    model.Id));
			});

            return Task.CompletedTask;
        }

        public async Task UpdateGangwar(int id, int ownerId)
        {
            var gangwarModel = _gangwarService.GetGangwar(id);
            if (gangwarModel == null) return;

            gangwarModel.OwnerId = ownerId;

            await _gangwarService.UpdateGangwar(gangwarModel);
        }

        public Task<GangwarModel> GetGangwarModel(int id)
        {
            return Task.FromResult(_gangwarService.Gangwar.FirstOrDefault(x => x.Id == id))!;
        }

        public Task<GangwarModel> GetRunningGangwarByTeam(int teamId)
        {
            return Task.FromResult(_gangwarService.Gangwar.FirstOrDefault(x => x.IsRunning && (x.OwnerId == teamId || x.AttackerId == teamId)))!;
        }

        public Task<bool> IsGangwarFull(int id, int teamId)
        {
            return Task.FromResult(ClPlayer.All.Where(x => x.DbModel.GangwarId == id && x.DbModel.Team == teamId).ToList().Count >= 15);
        }

        public async Task FlagTick()
        {
            for(var i = 0; i < _gangwarService.Gangwar.Count; i++)
            {
                if (!_gangwarService.Gangwar[i].IsRunning) continue;

                if(_gangwarService.Gangwar[i].LastAttacked.AddMinutes(45) <= DateTime.Now)
                {
                    await EndGangwar(_gangwarService.Gangwar[i].Id);
                    continue;
                }

                for(var j = 0; j < _gangwarService.Gangwar[i].Flags.Count; j++)
                {
                    var player = ClPlayer.All.FirstOrDefault(x => x.IsInGangwar && x.Dimension == _gangwarService.Gangwar[i].Id && x.Position.Distance(_gangwarService.Gangwar[i].Flags[j].FlagPosition) < 1.5f && x.DbModel.Alive);
                    if (player == null) continue;

                    if(player.DbModel.Team == _gangwarService.Gangwar[i].AttackerId)
                    {
                        _gangwarService.Gangwar[i].AttackerPoints += 2;
						continue;
                    }

					_gangwarService.Gangwar[i].DefenderPoints += 2;
				}

                await UpdateHudInGangwar(_gangwarService.Gangwar[i].Id);
            }
        }

        private async Task UpdateHudInGangwar(int gwId)
        {
            var gw = _gangwarService.GetGangwar(gwId);
            if (gw == null) return;

			var attacker = await _teamService.GetTeam(gw.AttackerId);
			var defender = await _teamService.GetTeam(gw.OwnerId);
            if (attacker == null || defender == null) return;

			foreach (var player in ClPlayer.All.Where(x => x.IsInGangwar && x.Dimension == gwId))
            {
                await player.EmitBrowser("Hud:UpdateGangwar", JsonConvert.SerializeObject(new GangwarHudModel(
                    new GangwarHudTeamModel("", "", gw.AttackerPoints),
					new GangwarHudTeamModel("", "", gw.DefenderPoints),
                    0)));
			}
		}

		public async Task OnJoin(ClPlayer player, int gwId)
        {
			var gw = _gangwarService.GetGangwar(gwId);
			if (gw == null) return;

			var attacker = await _teamService.GetTeam(gw.AttackerId);
			var defender = await _teamService.GetTeam(gw.OwnerId);
			if (attacker == null || defender == null) return;

			await player.EmitBrowser("Hud:ShowGangwar", true, JsonConvert.SerializeObject(new GangwarHudModel(
					new GangwarHudTeamModel(attacker.Name, attacker.ShortName, gw.AttackerPoints),
					new GangwarHudTeamModel(defender.Name, defender.ShortName, gw.DefenderPoints),
					(int)Math.Floor((gw.LastAttacked.AddMinutes(45) - DateTime.Now).TotalSeconds))));

            player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(1500), 5);
			player.DbModel.Inventory.AddItem(InventoryModule.GetItemBase(1501), 5);
		}

        public async Task PlayerDeath(ClPlayer player, IEntity? killer)
        {
			var gw = _gangwarService.GetGangwar(player.Dimension);
            if (gw == null) return;

			if (player.DbModel.Team == gw.AttackerId)
            {
				gw.DefenderPoints += 2;
                foreach (var p in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == gw.OwnerId))
                    await p.Notify("GANGWAR", "+2 Punkte für das töten eines Gegners.", NotificationType.INFO);
			}
			else
            {
				gw.AttackerPoints += 2;
				foreach (var p in ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Team == gw.AttackerId))
					await p.Notify("GANGWAR", "+2 Punkte für das töten eines Gegners.", NotificationType.INFO);
			}

            await UpdateHudInGangwar(gw.Id);

			var playerStats = gw.Leaderboard.FirstOrDefault(x => x.Name == player.Name);
			if (playerStats == null)
			{
				gw.Leaderboard.Add(new(player.Name, 0, 1));
			}
			else
			{
				playerStats.Deaths++;
			}

			if (killer == null || killer.Type != BaseObjectType.Player) return;
            var killerPlayer = (ClPlayer)killer;

            var killerStats = gw.Leaderboard.FirstOrDefault(x => x.Name == killerPlayer.Name);
            if(killerStats == null)
            {
                gw.Leaderboard.Add(new(player.Name, 1, 0));
            }
            else
            {
				killerStats.Kills++;
            }
		}
	}
}
