using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.BankRobbery.Interface
{
	public interface IBankHeistController
	{
		bool VaultLocked { get; }
		bool HasBeenAttacked { get; }
		Task<bool> StartRobbery(ClPlayer player);
		Task UnlockVault(ClPlayer player, bool ignoreDist = false);
	}
}