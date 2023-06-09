using System.ComponentModel.DataAnnotations.Schema;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.JumpPoint;

namespace Backend.Utils.Models.Database
{
    public class JumpPointModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public JumpPointPosition EntryPosition { get; set; }
		public JumpPointPosition ExitPosition { get; set; }
		public List<int> PlayerAccessList { get; set; }
        public List<int> FactionAccessList { get; set; }
		public string Ipl { get; set; }

        public bool Locked { get; set; }

        [NotMapped]
        public bool Crackable { get; set; } = false;
        [NotMapped]
        public DateTime CrackedTime { get; set; }
        [NotMapped]
        public Action<ClPlayer, JumpPointModel>? OnCrack { get; set; }
        [NotMapped]
        public Action<ClPlayer, JumpPointModel, bool>? OnInteract { get; set; }

        public JumpPointModel() {}

        public JumpPointModel(int id, JumpPointPosition entryPosition, JumpPointPosition exitPointPosition,
            List<int> playerAccess, List<int> factionAccess, string ipl)
        {
            Id = id;
            EntryPosition = entryPosition;
            ExitPosition = exitPointPosition;
            PlayerAccessList = playerAccess;
            FactionAccessList = factionAccess;
            Ipl = ipl;

            Locked = true;
        }
    }
}
