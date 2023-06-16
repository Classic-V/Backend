using AltV.Net.Data;
using AltV.Net.Enums;
using Backend.Controllers.Ban.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Services.Account.Interface;
using Backend.Services.Ban;
using Backend.Services.Ban.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;
using Newtonsoft.Json;

namespace Backend.Modules.Anticheat
{
	public class AnticheatModule : Module<AnticheatModule>
	{
		private IBanService _banService;
		public AnticheatModule(IEventController eventController, IBanService banService) : base("AnticheatModule")
		{
			_banService = banService;
			// PLAYER
			eventController.OnClient<uint>("Server:Anticheat:Weapon", Weapon);
			eventController.OnClient<uint, int, int>("Server:Anticheat:Ammo", Ammo);
			eventController.OnClient<int>("Server:Anticheat:Healkey", Healkey);
			eventController.OnClient<bool>("Server:Anticheat:Godmode", Godmode);
			eventController.OnClient<Position>("Server:Anticheat:Teleport", Teleport);
			eventController.OnClient<int, int, int>("Server:Anticheat:DamageModifier", DamageModifier);
			eventController.OnClient<ClPlayer, int>("Server:Anticheat:GodmodeTarget", GodmodeTarget);

			// VEHICLE
			eventController.OnClient("Server:Anticheat:VehicleParachute", VehicleParachute);
			eventController.OnClient("Server:Anticheat:RocketBoost", RocketBoost);
		}

		#region Player
		private void Weapon(ClPlayer player, string eventKey, uint weaponHash)
		{
			if (player.DbModel == null) return;

			BanPlayer(player, $"GiveWeapon (Weapon: {weaponHash})");
		}

		private void Ammo(ClPlayer player, string eventKey, uint weaponHash, int ammo, int allowedAmmo)
		{
			if (player.DbModel == null) return;

			BanPlayer(player, $"GiveAmmo (Weapon: {weaponHash} | Ammo: {ammo} | Allowed: {allowedAmmo})");
		}

		private void Healkey(ClPlayer player, string eventKey, int acHealth)
		{
			if (player.DbModel == null || player.Health + player.Armor <= acHealth) return;

			// Todo: add log
			Console.WriteLine($"[ANTICHEAT] {player.Name}: Healkey (Health: {player.Health + player.Armor} | Allowed: {acHealth})");
		}

		private void Godmode(ClPlayer player, string eventKey, bool allowedState)
		{
			if (player.DbModel == null || player.Invincible == allowedState) return;

			BanPlayer(player, $"Godmode (Value: {player.Invincible} | Allowed: {allowedState})");
		}

		private void Teleport(ClPlayer player, string eventKey, Position allowedPos)
		{
			if (player.DbModel == null) return;

			var dist = player.Position.Distance(allowedPos);

			Console.WriteLine($"[ANTICHEAT] {player.Name}: Teleport (Position: {JsonConvert.SerializeObject(player.Position)} Distance: {dist})");
		}

		private void DamageModifier(ClPlayer player, string eventKey, int weapon, int damage, int allowedDamage)
		{
			if (player.DbModel == null) return;

			BanPlayer(player, $"Damage Modifier (Weapon: {weapon} | Damage: {damage} | Allowed: {allowedDamage})");
		}

		private void GodmodeTarget(ClPlayer player, string eventKey, ClPlayer? target, int hp)
		{
			if (target == null || target.DbModel == null || (target.Health + target.Armor) <= hp) return;

			BanPlayer(target, $"Godmode (Health: {player.Health + player.Armor} | BeforeDamage: {hp})");
		}

		private void BanPlayer(ClPlayer player, string reason)
		{
			if (player.DbModel == null) return;
			
			if (player.DbModel.AdminRank >= AdminRank.SUPERADMINISTRATOR) return;

			// player.DbModel.Ban = new BanModel(true, reason, DateTime.Now, "ANTICHEAT");

			_banService.AddBan(new BanModel(player.DbModel.Id, "Anticheat Ban", permanent: true));
			player.Kick("Dein Account wurde gesperrt! Für weitere Informationen kannst du ein Ticket im Forum erstellen. (https://forum.classicv.de)");
			ClPlayer.All.ForEach(async x => await x.ShowGlobalNotify("ADMINISTRATIVE NACHRICHT", $"Der Spieler {player.Name} wurde vom Anticheat permanent aus der Community ausgeschlossen!", 5000));
		}
		#endregion

		#region Vehicle
		private void VehicleParachute(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null) return;

			BanPlayer(player, $"Vehicle Parachute");
		}

		private void RocketBoost(ClPlayer player, string eventKey)
		{
			if (player.DbModel == null) return;

			BanPlayer(player, $"Vehicle Rocketboost");
		}
		#endregion
	}
}