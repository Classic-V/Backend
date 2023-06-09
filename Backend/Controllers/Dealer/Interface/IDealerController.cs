using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Controllers.Dealer.Interface
{
	public interface IDealerController
	{
		Task LoadDealer(DealerModel model);
		Task OpenDealerMenu(ClPlayer player, int id);
		Task SellItem(ClPlayer player, int id, int itemId);
	}
}