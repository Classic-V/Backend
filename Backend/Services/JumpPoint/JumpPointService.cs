using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using Backend.Services.JumpPoint.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;

namespace Backend.Services.JumpPoint
{
    public class JumpPointService : IJumpPointService
    {
        public List<JumpPointModel> JumpPoints { get; private set; }

        public JumpPointService()
        {
            using var ctx = new DBContext();
            JumpPoints = new List<JumpPointModel>(ctx.JumpPoints);
        }

        public async Task AddJumpPoint(JumpPointModel jumpPoint, bool inDatabase)
        {
            JumpPoints.Add(jumpPoint);

            if (!inDatabase) return;

            await using var ctx = new DBContext();
            await ctx.JumpPoints.AddAsync(jumpPoint);
            await ctx.SaveChangesAsync();
        }

        public Task<JumpPointModel?> GetJumpPoint(int jumpPointId)
        {
            return Task.FromResult(JumpPoints.FirstOrDefault(x => x.Id == jumpPointId));
        }

        public Task RemoveLocalJumpPoint(int id)
        {
            var model = JumpPoints.FirstOrDefault(x => x.Id == id);
            if (model != null)
			{
				JumpPoints.Remove(model);
			}

            lock (ClShape.All)
            {
                foreach (var shape in ClShape.All.Where(x => x.Id == id && x.ShapeType == Utils.Enums.ColshapeType.JUMP_POINT))
                    shape.Destory();
			}

            return Task.CompletedTask;
        }
    }
}
