using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.MoneyTransportJob
{
    public class BankReturnData
    {
        public int BankId { get; set; }
        [NotMapped] public bool Returned { get; set; }

        public BankReturnData() { }
        public BankReturnData(int bankId)
        {
            BankId = bankId;
            Returned = false;
        }
    }
}
