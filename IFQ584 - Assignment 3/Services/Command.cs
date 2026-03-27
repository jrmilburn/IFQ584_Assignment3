namespace BoardGames
{
	public enum CommandType {Move, Undo, Redo, Save, Load, Help, Quit, Invalid}
	public class Command(CommandType type, string[] input)
	{
		public CommandType Type { get; } = type;
		public string[] Input { get; } = input;
		public static Command Parse(string playerInput)
		{
			if (string.IsNullOrWhiteSpace(playerInput))
				return new Command(CommandType.Invalid, []);
			string commandType = playerInput.Split(" ")[0].ToUpper();
			string[] inputData = playerInput.Split(" ")[1..];
			CommandType type;
			switch (commandType)
			{
				case "M" or "MOVE": type = CommandType.Move; break;
				case "U" or "UNDO": type = CommandType.Undo; break;
				case "R" or "REDO": type = CommandType.Redo; break;
				case "S" or "SAVE": type = CommandType.Save; break;
				case "L" or "LOAD": type = CommandType.Load; break;
				case "H" or "HELP": type = CommandType.Help; break;
				case "Q" or "QUIT": type = CommandType.Quit; break;
				default: type = CommandType.Invalid; break;
			}
			return new Command(type, inputData);
		}
		public static void ShowHelp()
		{

		}
	}
}
