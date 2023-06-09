using AltV.Net;
using AltV.Net.Elements.Entities;
using Backend.Utils.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Factories
{
	public class ColshapeFactory : IBaseObjectFactory<IColShape>
	{
		public IColShape Create(ICore core, nint baseObjectPointer)
		{
			return new ClShape(core, baseObjectPointer);
		}
	}
}