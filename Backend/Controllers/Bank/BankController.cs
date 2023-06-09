using System.Numerics;
using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Bank.Interface;
using Backend.Services.Bank.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Bank;

public class BankController : IBankController
{
    private readonly IBankService _bankService;

    public BankController(IBankService bankService)
    {
        _bankService = bankService;

        _bankService.Banks.ForEach(LoadBank);
    }

    private void LoadBank(BankModel bank)
    {
        if (bank.Type == BankType.BANK)
        {
            if (bank.Id == 13)
            {
                var bankShape = (ClShape)Alt.CreateColShapeSphere(bank.Position, 1.5f);
                bankShape.Dimension = 0;
                bankShape.Id = bank.Id;
                bankShape.ShapeType = ColshapeType.BANK;
                return;
            }

            var blip = Alt.CreateBlip(BlipType.Destination, bank.Position);
            blip.Name = bank.Name;
            blip.Dimension = 0;
            blip.Sprite = 108;
            blip.Color = 2;
            blip.ShortRange = true;
        }

        var shape = (ClShape)Alt.CreateColShapeSphere(bank.Position, 1.5f);
        shape.Dimension = 0;
        shape.Id = bank.Id;
        shape.ShapeType = ColshapeType.BANK;
    }

    public BankModel? GetBank(int id)
    {
        return _bankService.GetBankById(id);
    }
}