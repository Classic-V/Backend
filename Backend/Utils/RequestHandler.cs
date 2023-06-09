using Backend.Utils.Web;
using System.Net;
using System.Reflection;
using System.Web;

namespace Backend.Utils
{
	public class RequestHandler
	{
		private static readonly List<RequestParser> Requests = new();
		private static readonly List<RequestScript> LoadedScripts = new();
		private static List<string> _ipWhitelist = new();
		private static WebServer _webServer;
		private static string _key = "01234KLMNOPQRS";

		static RequestHandler() { }

		public static void Register()
		{
			lock (Requests)
			{
				foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
				{
					if (!type.IsSubclassOf(typeof(RequestScript)) || type.IsAbstract) continue;
					if (Activator.CreateInstance(type) is RequestScript script) LoadedScripts.Add(script);
				}

				LoadedScripts.ForEach(script =>
				{
					var methods = script.GetType().GetMethods();
					var requests = methods.Where(x => x.CustomAttributes.Any(attr => attr.AttributeType == typeof(RequestAttribute)));
					foreach (var request in requests)
					{
						var req = request.GetCustomAttribute<RequestAttribute>();
						var args = request.GetParameters();

						var parser = new RequestParser
						{
							Request = string.IsNullOrWhiteSpace(req.RequestString) ? request.Name.ToLower() : req.RequestString,
							RequestParams = args.Length,
							Parameters = args,
							Method = request
						};

						lock (Requests) Requests.Add(parser);

						Console.WriteLine($"Registered Request: {script.GetType().Name}:{request.Name}.");
					}
				});
			}

			WebserverStart();
		}

		public static void Unregister()
		{
			Console.WriteLine("Starting to unregister requests.");

			lock (Requests)
			{
				Requests.Clear();
				_ipWhitelist.Clear();
			}

			WebserverStop();
			Console.WriteLine("Finished to unregister requests.");
		}

		private static bool Parse(string request, Dictionary<string, string> parameters)
		{
			var result = false;

			lock (Requests)
			{
				var requests = Requests.Where(x => x.Request == request).ToList();
				foreach (var req in requests)
					result = result || req.Parse(parameters);
			}

			return result;
		}

		private static void WebserverStart()
		{
			var host = FetchPublicIp().Result;

			if (host == null) return;

			try
			{
				host = $"http://{host}:19800/";
				_webServer = new WebServer(HandleRequest, host);
				_webServer.Run();
				Console.WriteLine("Webserver is listening on Port {0}!", host);
			}
			catch (Exception)
			{
				Console.WriteLine("Port 19800 is not allowed for your public IP.");
			}
		}

		private static async Task<string> FetchPublicIp()
		{
			using var client = new HttpClient();
			try
			{
				var response = await client.GetStringAsync("http://ipinfo.io/ip");
				return response.Trim();
			}
			catch (HttpRequestException e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		private static void WebserverStop()
		{
			if (_webServer != null) _webServer.Stop();
		}

		private static string HandleRequest(HttpListenerRequest request)
		{
			// request.RemoteEndPoint.Address.ToString() != "51.38.98.124"
			if (request == null || request.HttpMethod != "POST") return "METHOD NOT ALLOWED";

			var data = GetPostData(request);

			data.Remove("secret", out var secretKey);
			if (_key != "" && secretKey != _key) return "UNAUTHORIZED";
			if(request.RawUrl == null || !Parse(request.RawUrl, data)) return "BAD REQUEST";

			return "OK";
		}

		private static Dictionary<string, string> GetPostData(HttpListenerRequest requst)
		{
			var values = new Dictionary<string, string>();

			using (var stream = requst.InputStream)
			using (var reader = new StreamReader(stream, requst.ContentEncoding))
			{
				try
				{
					foreach (var part in reader.ReadToEnd().Split("&"))
					{
						var text = HttpUtility.UrlDecode(part);
						if (text == null) continue;
						var split = text.Split("=");
						values.TryAdd(split[0], split[1]);
					}
				}
				catch
				{
					Console.WriteLine("Error while getting post data");
				}
			}

			return values;
		}

	}
}