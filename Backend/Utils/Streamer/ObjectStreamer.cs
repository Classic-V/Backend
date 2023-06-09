using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Utils.Streamer
{
	public static class ObjectStreamer
	{
		public static readonly List<Models.Object> Objects = new();

		public static int AddObject(Models.Object obj)
		{
			Objects.Add(obj);
			AltAsync.EmitAllClients("Client:ObjectStreamer:AddObject", JsonConvert.SerializeObject(obj));
			return obj.Id;
		}

		public static int AddObject(uint hash, Position position, Rotation rotation, bool network, bool dynamic, int dimension)
		{
			var obj = new Models.Object(hash, position, rotation, network, dynamic, dimension);
			Objects.Add(obj);
			AltAsync.EmitAllClients("Client:ObjectStreamer:AddObject", JsonConvert.SerializeObject(obj));
			return obj.Id;
		}

		public static void RemoveObject(int id)
		{
			var obj = Objects.FirstOrDefault(x => x.Id == id);
			if (obj == null) return;

			Objects.Remove(obj);
			AltAsync.EmitAllClients("Client:ObjectStreamer:RemoveObject", id);
		}
	}
}