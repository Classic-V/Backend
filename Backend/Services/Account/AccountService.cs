using Backend.Services.Account.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Account
{
	public class AccountService : IAccountService
	{
		public List<AccountModel> Accounts { get; private set; } = new();

		public AccountService()
		{
			using var ctx = new DBContext();

			Accounts = new List<AccountModel>(ctx.Accounts);
		}

		public AccountModel? GetAccount(int id)
		{
			return Accounts.FirstOrDefault(x => x.Id == id);
		}

		public AccountModel? GetAccount(string name)
		{
			return Accounts.FirstOrDefault(x => x.Name.ToLower() == name.ToLower());
		}

		public async Task AddAccount(AccountModel account)
		{
			await using var ctx = new DBContext();

			Accounts.Add(account);
			await ctx.Accounts.AddAsync(account);
			await ctx.SaveChangesAsync();
		}

        public async Task UpdateAccount(AccountModel account)
        {
            using var ctx = new DBContext();

            ctx.Accounts.Update(account);
            await ctx.SaveChangesAsync();
        }
	}
}