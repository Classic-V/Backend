using AltV.Net.Elements.Entities;
using Backend.Controllers.Bank.Interface;
using Backend.Controllers.Event.Interface;
using Backend.Data;
using Backend.Services.Bank.Interface;
using Backend.Services.MoneyTransportJob.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Newtonsoft.Json;

namespace Backend.Modules.Bank;

public class BankModule : Module<BankModule>, IEventColshape
{
    private readonly IBankController _bankController;
    private readonly IBankService _bankService;
    private readonly IMoneyTransportJobService _moneyTransportJobService;

    public BankModule(IBankController bankController, IBankService bankService, IEventController eventController, IMoneyTransportJobService moneyTransportJobService) : base("Bank")
    {
        _bankController = bankController;
        _bankService = bankService;
        _moneyTransportJobService = moneyTransportJobService;

        eventController.OnClient<int>("Server:Bank:Open", OpenBank);
        eventController.OnClient<int>("Server:Bank:Deposit", Deposit);
        eventController.OnClient<int>("Server:Bank:Withdraw", Withdraw);
    }

    private async void OpenBank(ClPlayer player, string eventKey, int id)
    {
        if (player.DbModel == null) return;

        if (_moneyTransportJobService.MoneyTransportJobs.Where(x => x.RouteOwner == player.DbModel.Id).Count() > 0) return;

        var bank = _bankController.GetBank(id);
        if (bank == null) return;

        var bankData = new
		{
			Type = bank.Type,
			Balance = player.DbModel.BankMoney,
        };

        await player.ShowComponent("Bank", true, JsonConvert.SerializeObject(bankData));
    }

    private async void Deposit(ClPlayer player, string eventKey, int money)
    {
        if (player.DbModel == null || player.DbModel.Money < money || money <= 0) return;

        var bank = _bankService.Banks.FirstOrDefault(x => player.Position.Distance(x.Position) < 3);
        if (bank == null) return;

        await player.RemoveMoney(money);
        player.DbModel.BankMoney += money;

        await player.Notify("Bank", $"Du hast ${money} eingezahlt.", NotificationType.SUCCESS);
        await player.CreateTransactionHistory(bank.Name, TransactionType.DEPOSIT, money);
    }

    private async void Withdraw(ClPlayer player, string eventKey, int money)
	{
		if (player.DbModel == null || player.DbModel.BankMoney < money || money <= 0) return;

		var bank = _bankService.Banks.FirstOrDefault(x => player.Position.Distance(x.Position) < 3);
		if (bank == null) return;

        player.DbModel.BankMoney -= money;
        await player.AddMoney(money);

        await player.Notify("Bank", $"Du hast ${money} abghoben.", NotificationType.SUCCESS);
        await player.CreateTransactionHistory(bank.Name, TransactionType.WITHDRAW, money);
    }

    public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
    {
        if (entity.Type != BaseObjectType.Player || shape == null || shape.ShapeType != ColshapeType.BANK) return;

        ClPlayer player = (ClPlayer)entity;

        player.SetInteraction(Interactions.KEY_E, Interactions.E_MONEY_TRANSPORT_JOB_RETURN);
    }
}