using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.PizzaDeliveryJob
{
    public class DeliveryReturnData
    {
        public int HouseId { get; set; }
        [NotMapped] public bool IsDelivered { get; set; }

        public DeliveryReturnData() { }
        public DeliveryReturnData(int houseId)
        {
            HouseId = houseId;
        }
    }
}
