using Backend.Utils.Enums;

namespace Backend.Utils.Models.Bank;

public class TransactionHistoryModel
{
    public string Name { get; set; }
    public TransactionType Type { get; set; }
    public int Amount { get; set; }
    public DateTime DateTime { get; set; }

    public TransactionHistoryModel() {}
    public TransactionHistoryModel(string name, TransactionType type, int amount, DateTime dateTime)
    {
        Name = name;
        Type = type;
        Amount = amount;
        DateTime = dateTime;
    }
}