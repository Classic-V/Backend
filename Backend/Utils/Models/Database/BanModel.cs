namespace Backend.Utils.Models.Database;

public class BanModel
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public bool Permanent { get; set; }
    public bool Active { get; set; }
    public DateTime BannedFrom { get; set; }
    public DateTime BannedTill { get; set; }
    public string Reason { get; set; }

    public BanModel() {}

    public BanModel(int accountId, string reason, bool permanent = false, int timeInDays = 1)
    {
        Active = true;
        AccountId = accountId;
        Permanent = permanent;
        BannedFrom = DateTime.Now;
        BannedTill = DateTime.Now.AddDays(timeInDays);
        Reason = reason;
    }
}