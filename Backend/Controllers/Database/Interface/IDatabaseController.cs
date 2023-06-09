using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Database.Interface
{
	public interface IDatabaseController
	{
		Task SaveDatabase();
	}
}