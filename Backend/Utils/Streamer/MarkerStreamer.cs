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
	public static class MarkerStreamer
	{
		public static readonly List<Models.Marker> Markers = new();

		public static int AddMarker(Models.Marker obj)
		{
			Markers.Add(obj);
			AltAsync.EmitAllClients("Client:MarkerStreamer:AddMarker", JsonConvert.SerializeObject(obj));
			return obj.Id;
		}

		public static void RemoveMarker(int id)
		{
			var obj = Markers.FirstOrDefault(x => x.Id == id);
			if (obj == null) return;

			Markers.Remove(obj);
			AltAsync.EmitAllClients("Client:MarkerStreamer:RemoveMarker", id);
		}
	}
}