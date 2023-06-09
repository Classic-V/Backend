using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Utils.Models.Database;

namespace Backend.Services.Storage.Interface
{
    public interface IStorageService
    {
        List<StorageModel> Storages { get; }
        Task AddStorage(StorageModel model);
		Task UpdateStorage(StorageModel model);
		Task UpdateStorages();
		Task<StorageModel?> GetStorage(int storageId);
		Task<bool> HasStorage(int ownerId);
	}
}
