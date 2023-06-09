using Backend.Utils.Models.Database;

namespace Backend.Services.Bank.Interface;

public interface IBankService
{
    List<BankModel> Banks { get; }

    BankModel GetBankById(int id);
    Task AddBank(BankModel bank);
    Task UpdateBank(BankModel bank);
}