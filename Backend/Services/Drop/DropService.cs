using Backend.Services.Drop.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Drop
{
	public class DropService : IDropService
	{
		public List<DropModel> Drops { get; private set; } = new();

		public DropService()
		{
			using var ctx = new DBContext();

			Drops = new List<DropModel>(ctx.Drops);
		}

		public DropModel? GetDrop(int id)
		{
			return Drops.FirstOrDefault(x => x.Id == id);
		}

		public async Task<int> AddDrop(DropModel drop)
		{
			await using var ctx = new DBContext();

			Drops.Add(drop);
			await ctx.Drops.AddAsync(drop);
			await ctx.SaveChangesAsync();
			return drop.Id;
		}

		public async Task UpdateDrop(DropModel drop)
		{
			await using var ctx = new DBContext();

			ctx.Drops.Update(drop);
			await ctx.SaveChangesAsync();
		}
	}
}