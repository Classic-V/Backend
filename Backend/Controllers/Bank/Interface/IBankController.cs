using Backend.Utils.Models.Database;

namespace Backend.Controllers.Bank.Interface;

public interface IBankController
{
    BankModel? GetBank(int id);
}