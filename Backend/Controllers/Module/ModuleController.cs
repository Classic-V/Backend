using System.Timers;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Module.Interface;
using Backend.Controllers.Timer.Interface;
using Backend.Utils;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Module;

public class ModuleController : IModuleController
{
	private readonly IEnumerable<IEventModuleLoad> _moduleLoadEvents;
	private readonly IEnumerable<IEventPlayerConnect> _playerConnectEvents;
	private readonly IEnumerable<IEventPlayerDisconnect> _playerDisconnectEvents;
	private readonly IEnumerable<IEventPlayerDeath> _playerDeathEvents;
	private readonly IEnumerable<IEventPlayerWeaponSwitch> _playerWeaponSwitchEvents;
	private readonly IEnumerable<IEventPlayerDamage> _playerDamageEvents;
	private readonly IEnumerable<IEventColshape> _colshapeEvents;
	private readonly IEnumerable<IEventVehicleEnter> _vehicleEnterEvents;
	private readonly IEnumerable<IEventVehicleExit> _vehicleExitEvents;

	private readonly IEnumerable<IEventIntervalFiveSeconds> _intervalFiveSeconds;
    private readonly IEnumerable<IEventIntervalThirtySecond> _intervalThirtySeconds;
	private readonly IEnumerable<IEventIntervalMinute> _intervalMinute;
    private readonly IEnumerable<IEventIntervalTenMinute> _intervalTenMinute;

    public ModuleController(
        IEnumerable<IEventModuleLoad> moduleLoadEvents,
        IEnumerable<IEventPlayerConnect> playerConnectEvents,
        IEnumerable<IEventPlayerDisconnect> playerDisconnectEvents,
        IEnumerable<IEventPlayerDeath> playerDeathEvents,
        IEnumerable<IEventPlayerWeaponSwitch> playerWeaponSwitchEvents,
        IEnumerable<IEventPlayerDamage> playerDamageEvents,
        IEnumerable<IEventColshape> colshapeEvents,
        IEnumerable<IEventVehicleEnter> vehicleEnterEvents,
        IEnumerable<IEventVehicleExit> vehicleExitEvents,

		IEnumerable<IEventIntervalFiveSeconds> intervalFiveSeconds,
        IEnumerable<IEventIntervalThirtySecond> intervalThirtySeconds,
		IEnumerable<IEventIntervalMinute> intervalMinute,
        IEnumerable<IEventIntervalTenMinute> intervalTenMinute,
        
        ITimerController timerController)
    {
        _moduleLoadEvents = moduleLoadEvents;
        _playerConnectEvents = playerConnectEvents;
        _playerDisconnectEvents = playerDisconnectEvents;
        _playerDeathEvents = playerDeathEvents;
        _playerWeaponSwitchEvents = playerWeaponSwitchEvents;
        _playerDamageEvents = playerDamageEvents;
        _colshapeEvents = colshapeEvents;
        _vehicleEnterEvents = vehicleEnterEvents;
        _vehicleExitEvents = vehicleExitEvents;

        _intervalFiveSeconds = intervalFiveSeconds;
        _intervalThirtySeconds = intervalThirtySeconds;
		_intervalMinute = intervalMinute;
        _intervalTenMinute = intervalTenMinute;

        AltAsync.OnPlayerConnect += OnPlayerConnect;
        AltAsync.OnPlayerDisconnect += OnPlayerDisconnect;
        AltAsync.OnPlayerDead += OnPlayerDeath;
        AltAsync.OnPlayerWeaponChange += OnPlayerWeaponSwitch;
        AltAsync.OnPlayerDamage += OnPlayerDamage;
        AltAsync.OnColShape += OnColshape;
        AltAsync.OnPlayerEnterVehicle += OnPlayerEnterVehicle;
        AltAsync.OnPlayerLeaveVehicle += OnPlayerExitVehicle;

		timerController.Add(5000, OnEveryFiveSeconds);
		timerController.Add(30000, OnEveryThirtySeconds);
		timerController.Add(60 * 1000, OnEveryMinute);
        timerController.Add(10 * 60 * 1000, OnEveryTenMinute);
    }

    public async Task Load()
    {
        foreach (var module in _moduleLoadEvents)
        {
            if (module.Enabled)
                await module.OnLoad();
        }
    }

    private async Task OnPlayerConnect(IPlayer player, string reason)
    {
        foreach (var module in _playerConnectEvents)
        {
            if (module.Enabled)
                await module.OnPlayerConnect((ClPlayer)player, reason);
        }
    }
    
    private async Task OnPlayerDisconnect(IPlayer player, string reason)
    {
        foreach (var module in _playerDisconnectEvents)
        {
            if (module.Enabled)
                await module.OnPlayerDisconnect((ClPlayer)player, reason);
        }
    }

    private async Task OnPlayerDeath(IPlayer player, IEntity killer, uint weapon)
    {
        foreach (var module in _playerDeathEvents)
        {
            if (module.Enabled)
                await module.OnPlayerDeath((ClPlayer)player, killer, weapon);
        }
    }
    
    private async Task OnPlayerWeaponSwitch(IPlayer player, uint oldWeapon, uint newWeapon)
    {
        foreach (var module in _playerWeaponSwitchEvents)
        {
            if (module.Enabled)
                await module.OnPlayerWeaponSwitch((ClPlayer)player, oldWeapon, newWeapon);
        }
    }
    
    private async Task OnPlayerDamage(IPlayer player, IEntity attacker, ushort oldHealth, ushort oldArmor,
        ushort oldMaxHealth, ushort oldMaxArmor,  uint weapon, ushort healthDamage, ushort armorDamage)
    {
        foreach (var module in _playerDamageEvents)
        {
            if (module.Enabled)
                await module.OnPlayerDamage((ClPlayer)player, attacker, oldHealth, oldArmor, oldMaxHealth, oldMaxArmor, weapon, healthDamage, armorDamage);
        }
    }
    
    private async Task OnColshape(IColShape shape, IEntity entity, bool entered)
    {
        foreach (var module in _colshapeEvents)
        {
            if (module.Enabled)
                await module.OnColshape((ClShape)shape, entity, entered);
        }
    }
    
    private async Task OnPlayerEnterVehicle(IVehicle vehicle, IPlayer player, byte seat)
    {
        foreach (var module in _vehicleEnterEvents)
            if (module.Enabled)
                await module.OnVehicleEnter((ClVehicle)vehicle, (ClPlayer)player, seat);
    }
    
    private async Task OnPlayerExitVehicle(IVehicle vehicle, IPlayer player, byte seat)
    {
        foreach (var module in _vehicleExitEvents)
            if (module.Enabled)
                await module.OnVehicleExit((ClVehicle)vehicle, (ClPlayer)player, seat);
    }

	private async void OnEveryFiveSeconds(object? obj, ElapsedEventArgs args)
	{
		foreach (var module in _intervalFiveSeconds)
			if (module.Enabled)
				await module.OnEveryFiveSeconds();
	}

	private async void OnEveryThirtySeconds(object? obj, ElapsedEventArgs args)
	{
		foreach (var module in _intervalThirtySeconds)
			if (module.Enabled)
				await module.OnEveryThirtySeconds();
	}

	private async void OnEveryMinute(object? obj, ElapsedEventArgs args)
    {
        foreach (var module in _intervalMinute)
            if (module.Enabled)
                await module.OnEveryMinute();
    }
    
    private async void OnEveryTenMinute(object? obj, ElapsedEventArgs args)
    {
        foreach (var module in _intervalTenMinute)
            if (module.Enabled)
                await module.OnEveryTenMinute();
    }
}