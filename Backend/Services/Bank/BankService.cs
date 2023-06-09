using Backend.Services.Bank.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Bank;

public class BankService : IBankService
{
    public List<BankModel> Banks { get; }

    public BankService()
    {
        var ctx = new DBContext();

        Banks = new List<BankModel>(ctx.Bank);
    }

    public BankModel GetBankById(int id)
    {
        return Banks.FirstOrDefault(bank => bank.Id == id)!;
    }

    public async Task AddBank(BankModel bank)
    {
        Banks.Add(bank);

        var ctx = new DBContext();
        ctx.Bank.Add(bank);
        await ctx.SaveChangesAsync();
    }

    public async Task UpdateBank(BankModel bank)
    {
        var ctx = new DBContext();
        ctx.Bank.Update(bank);
        await ctx.SaveChangesAsync();
    }
}