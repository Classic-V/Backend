using System.Numerics;
using AltV.Net;
using AltV.Net.Data;
using Backend.Controllers.Jail.Interface;
using Backend.Services.Crime.Interface;
using Backend.Services.Jail.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Controllers.Jail;

public class JailController : IJailController
{
    private readonly IJailService _jailService;
    private readonly ICrimeService _crimeService;

    public JailController(IJailService jailService, ICrimeService crimeService)
    {
        _jailService = jailService;
        _crimeService = crimeService;

        _jailService.Jails.ForEach(LoadJail);
    }

    public void LoadJail(JailModel model)
    {
        var shape = (ClShape)Alt.CreateColShapeSphere(model.Position, model.Scale);
        shape.Id = model.Id;
        shape.Size = model.Scale;
        shape.Dimension = 0;
        shape.ShapeType = ColshapeType.JAIL;

        model.ImprisonPoint.ForEach(item =>
        {
            var imprisonShape = (ClShape)Alt.CreateColShapeSphere(item, 2f);
            imprisonShape.Id = model.Id;
            imprisonShape.Size = 1.5f;
            imprisonShape.Dimension = 0;
            imprisonShape.ShapeType = ColshapeType.JAIL_IMPRISON;
        });
    }

    public async Task<JailModel?> GetJail(int id)
    {
        return await _jailService.GetJail(id);
    }

    public async Task<int> GetPlayerJailDuration(int id)
    {
        int time = 0;

        ClPlayer prisoner = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == id)!;
        if (prisoner == null!) return -1;

        prisoner.DbModel.Crimes.ForEach(async crime =>
        {
            var crimeData = await _crimeService.GetCrimeData(crime.CrimeId);
            if (crimeData == null) return;

            time += crimeData.JailTime;
        });

        return time;
    }

    public async Task<int> GetPlayerJailFine(int id)
    {
        int price = 0;

        ClPlayer prisoner = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == id)!;
        if (prisoner == null!) return -1;

        prisoner.DbModel.Crimes.ForEach(async crime =>
        {
            var crimeData = await _crimeService.GetCrimeData(crime.CrimeId);
            if (crimeData == null) return;

            price += crimeData.Fine;
        });

        return price;
    }

    public async Task<bool> PlayerHasJailCrimes(int id)
    {
        int price = 0;

        ClPlayer prisoner = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == id)!;
        if (prisoner == null!) return false;

        prisoner.DbModel.Crimes.ForEach(async crime =>
        {
            var crimeData = await _crimeService.GetCrimeData(crime.CrimeId);
            if (crimeData == null) return;

            price += crimeData.Fine;
        });

        return price > 0;
    }
}