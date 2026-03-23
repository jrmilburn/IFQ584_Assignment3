namespace BoardGames
{
	public class Command(CommandType type, string[] args)
	{
		public CommandType Type { get; } = type; public string[] Args { get; } = args;
	}
}