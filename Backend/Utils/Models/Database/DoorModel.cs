using AltV.Net.Data;
using Backend.Utils.Enums;
using Backend.Utils.Models.Door;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
    public class DoorModel
    {
        public int Id { get; set; }
        public Position Position { get; set; }
        public List<DoorData> Doors { get; set; }
        public DoorType Type { get; set; }
        public bool Locked { get; set; }
        public float Radius { get; set; }
        public List<int> Access { get; set; }

        public DoorModel(Position position, List<DoorData> doors, DoorType type, bool locked, float radius, List<int> access)
        {
            Position = position;
            Doors = doors;
            Type = type;
            Locked = locked;
            Radius = radius;
            Access = access;
        }
    }
}
