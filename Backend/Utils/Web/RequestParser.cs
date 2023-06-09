using AltV.Net.Elements.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Web
{
	public class RequestParser
	{
		public string Request;
		public int RequestParams;
		public ParameterInfo[] Parameters;
		public MethodInfo Method;

		public bool Parse(Dictionary<string, string> parameters)
		{
			if (parameters.Count != RequestParams)
			{
				Console.WriteLine($"{Method}: Parameter-Count stimmt nicht überein.");
				return false;
			}

			var arguments = new object[parameters.Count];
			for (var i = 0; i < parameters.Count; i++)
			{
				if (Parameters[i].ParameterType == typeof(Player))
				{
					Console.WriteLine($"{Method}: Der Typ Client darf nicht als Parameter verwendet werden.");
					return false;
				}

				if (Parameters[i].ParameterType.IsEnum)
				{
					object enumOut;
					try
					{
						enumOut = Enum.Parse(Parameters[i].ParameterType, parameters.First(x => x.Key == Parameters[i].Name).Value, true);
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
						return false;
					}

					arguments[i] = enumOut;
					continue;
				}

				try
				{
					arguments[i] = Convert.ChangeType(parameters.First(x => x.Key == Parameters[i].Name).Value, Parameters[i].ParameterType, CultureInfo.InvariantCulture);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
					return false;
				}
			}

			try
			{
				Method.Invoke(null, arguments);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return false;
			}

			return true;
		}
	}
}