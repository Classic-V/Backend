using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Account.Interface
{
	public interface IAccountService
	{
		List<AccountModel> Accounts { get; }

		AccountModel? GetAccount(int id);
		AccountModel? GetAccount(string name);
		Task AddAccount(AccountModel account);
        Task UpdateAccount(AccountModel account);
    }
}