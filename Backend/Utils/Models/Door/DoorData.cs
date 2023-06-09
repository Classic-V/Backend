using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Door
{
    public class DoorData
    {
        public int Model { get; set; }
        public Position Position { get; set; }

        public DoorData(int model, Position position)
        {
            Model = model;
            Position = position;
        }
    }
}
