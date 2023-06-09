using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.JumpPoint
{
    public class JumpPointPosition
    {
        public Position Position { get; set; }
        public Rotation Rotation { get; set; }
        public float Radius { get; set; }
        public int Dimension { get; set; }

        public JumpPointPosition(Position position, Rotation rotation, float radius, int dimension)
        {
            Position = position;
            Rotation = rotation;
            Radius = radius;
            Dimension = dimension;
        }
    }
}
