using Backend.Utils.Web;

namespace Backend.ACP.Player
{
	public class KickHandler : RequestScript
	{
		[Request("/kick")]
		public static void Kick(string data)
		{
			Console.WriteLine("POST kick: " + data);
		}
	}
}