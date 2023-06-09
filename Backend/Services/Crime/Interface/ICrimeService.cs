using Backend.Utils.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Services.Crime.Interface
{
	public interface ICrimeService
	{
		List<CrimeModel> Crimes { get; }
		string CrimesJSON { get; }

		Task<CrimeModel?> GetCrimeData(int id);
		Task AddCrime(CrimeModel model);
	}
}