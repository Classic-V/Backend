using AltV.Net.Async;
using AltV.Net.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Streamer
{
	public static class PedStreamer
	{
		public static readonly List<Models.Ped> Peds = new();

		public static int AddObject(Models.Ped obj)
		{
			Peds.Add(obj);
			AltAsync.EmitAllClients("Client:PedStreamer:AddPed", JsonConvert.SerializeObject(obj));
			return obj.Id;
		}

		public static void RemoveObject(int id)
		{
			var obj = Peds.FirstOrDefault(x => x.Id == id);
			if (obj == null) return;

			Peds.Remove(obj);
			AltAsync.EmitAllClients("Client:PedStreamer:RemovePed", id);
		}
	}
}