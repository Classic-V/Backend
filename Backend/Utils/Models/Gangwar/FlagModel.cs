using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltV.Net.Data;
using Backend.Utils.Models.Entities;
using Newtonsoft.Json;

namespace Backend.Utils.Models.Gangwar
{
    public class FlagModel
    {
        public Position FlagPosition { get; set; }

        [JsonIgnore]
        public int FlagOwner { get; set; }

		[JsonIgnore]
        public ClShape? FlagShape { get; set; }

		[JsonIgnore]
        public int FlagMarkerId { get; set; }

        public FlagModel() {}
        public FlagModel(int flagOwner, Position flagPosition, float flagRadius)
        {
            FlagOwner = flagOwner;
            FlagPosition = flagPosition;
        }
    }
}
