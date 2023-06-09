using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Drop.Interface
{
	public interface IDropService
	{
		List<DropModel> Drops { get; }

		DropModel? GetDrop(int id);

		Task<int> AddDrop(DropModel drop);
		Task UpdateDrop(DropModel drop);
	}
}