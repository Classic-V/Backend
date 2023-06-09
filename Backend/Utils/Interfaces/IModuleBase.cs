namespace Backend.Utils.Interfaces
{
	public interface IModuleBase
	{
		string Name { get; }
		bool Enabled { get; }
	}
}