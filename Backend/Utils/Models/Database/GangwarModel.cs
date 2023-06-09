using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Gangwar;

namespace Backend.Utils.Models.Database
{
    public class GangwarModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int OwnerId { get; set; }
        public Position AttackPosition { get; set; }
        public float Radius { get; set; }
        public List<FlagModel> Flags { get; set; } = new();
        public DateTime LastAttacked { get; set; }
        public PositionModel AttackerSpawnPosition { get; set; } = new();
		public PositionModel DefenderSpawnPosition { get; set; } = new();
		public List<PositionModel> AttackerVehicleSpawnPosition {get; set; } = new();
		public List<PositionModel> DefenderVehicleSpawnPosition {get; set; } = new();

		[NotMapped]
        public ClShape? ColShape { get; set; }
        [NotMapped]
		public ClShape? AttackerShape { get; set; }
        [NotMapped]
        public int AttackerMarker { get; set; }
		[NotMapped]
		public ClShape? DefenderShape { get; set; }
		[NotMapped]
		public int DefenderMarker { get; set; }

		[NotMapped]
        public int MarkerId { get; set; }

        [NotMapped]
        public bool IsRunning { get; set; }

        [NotMapped]
        public int AttackerId { get; set; }
        [NotMapped]
        public int AttackerPoints { get; set; }
        [NotMapped]
        public int DefenderPoints { get; set; }

        [NotMapped]
        public List<LeaderboardModel> Leaderboard { get; set; } = new();
    }
}
