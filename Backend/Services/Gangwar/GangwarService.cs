using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backend.Services.Gangwar.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.Gangwar
{
    public class GangwarService : IGangwarService
    {
        public List<GangwarModel> Gangwar { get; private set; }

        public GangwarService()
        {
            var ctx = new DBContext();
            Gangwar = new List<GangwarModel>(ctx.Gangwars);
        }

        public GangwarModel GetGangwar(int id)
        {
            return Gangwar.FirstOrDefault(x => x.Id == id)!;
        }

        public async Task AddGangwar(GangwarModel model)
        {
            Gangwar.Add(model);

            var ctx = new DBContext();
            ctx.Gangwars.Add(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateGangwar(GangwarModel model)
        {
            var ctx = new DBContext();
            ctx.Gangwars.Update(model);
            await ctx.SaveChangesAsync();
        }

        public async Task UpdateGangwars()
        {
            for(var i = 0; i < Gangwar.Count; i++)
            {
                await UpdateGangwar(Gangwar[i]);
            }
        }
    }
}
