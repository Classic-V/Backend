using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Dealer.Interface
{
	public interface IDealerService
	{
		List<DealerModel> Dealer { get; }

		DealerModel? GetDealer(int id);
		Task AddDealer(DealerModel model);
	}
}