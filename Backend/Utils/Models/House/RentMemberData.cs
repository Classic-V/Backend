namespace Backend.Utils.Models.House;

public class RentMemberData
{
    public int Id { get; set; }
    public int RentPrice { get; set; }

    public RentMemberData() {}

    public RentMemberData(int id, int rentPrice)
    {
        Id = id;
        RentPrice = rentPrice;
    }
}