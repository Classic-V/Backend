using AltV.Net.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Database
{
    public class BarberModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Position Position { get; set; }
        public float Radius { get; set; }

        public BarberModel() { }

        public BarberModel(string name, Position position, float radius)
        {
            Name = name;
            Position = position;
            Radius = radius;
        }
    }
}
