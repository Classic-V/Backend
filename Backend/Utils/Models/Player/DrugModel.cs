using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Models.Player
{
	public class DrugModel
	{
		public DrugState MDMA { get; set; }
		public DrugState Meth { get; set; }
		public DrugState Ecstasy { get; set; }
		public DrugState Heroin { get; set; }

		public bool AnyDrugActive => MDMA.Active || Meth.Active || Ecstasy.Active || Heroin.Active;

		public DrugModel()
		{
			var expired = DateTime.Now.AddDays(-1);
			MDMA = new DrugState(false, expired);
			Meth = new DrugState(false, expired);
			Ecstasy = new DrugState(false, expired);
			Heroin = new DrugState(false, expired);
		}
	}
}