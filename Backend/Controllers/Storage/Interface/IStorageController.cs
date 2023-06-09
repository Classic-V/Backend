using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Inventory;

namespace Backend.Controllers.Storage.Interface
{
    public interface IStorageController
    {

        void LoadStorage(StorageModel model);
        void BuyStorage(ClPlayer player, int storageId);
        void SellStorage(ClPlayer player, int storageId, ClPlayer? buyer = null);
        Task CreateStorage(StorageModel model);
        Task AddInventory(StorageModel model);
        Task LoadInventory(StorageModel model, InventoryModel inventory);
        Task Upgrade(StorageModel model);
	}
}
