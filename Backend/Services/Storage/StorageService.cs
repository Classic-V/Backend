using Backend.Services.Storage.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Services.Storage
{
    public class StorageService : IStorageService
    {
        public List<StorageModel> Storages { get; private set; }

        public StorageService()
        {
            using var ctx = new DBContext();
            Storages = new List<StorageModel>(ctx.Storages);
        }

        public async Task AddStorage(StorageModel model)
        {
            await using var ctx = new DBContext();

			Storages.Add(model);
			await ctx.Storages.AddAsync(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateStorage(StorageModel model)
        {
            await using var ctx = new DBContext();

            ctx.Storages.Update(model);
            await ctx.SaveChangesAsync();
        }

		public async Task UpdateStorages()
		{
			await using var ctx = new DBContext();

			ctx.Storages.UpdateRange(Storages);
			await ctx.SaveChangesAsync();
		}

		public Task<StorageModel?> GetStorage(int storageId)
        {
            return Task.FromResult(Storages.FirstOrDefault(x => x.Id == storageId));
        }

        public Task<bool> HasStorage(int ownerId)
        {
			return Task.FromResult(Storages.FirstOrDefault(x => x.OwnerId == ownerId) != null);
		}
	}
}
