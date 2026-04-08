namespace TicTacToe_Framework
{
	public enum CommandType {Move, Undo, Redo, Save, Load, Help, Quit, Invalid} // Command type constants available in the program
	public class Command(CommandType type, string[] input) // Player will type input on their turn which will be converted into a command to allow the user to input their turn in 1 line
	{
		public CommandType Type { get; } = type;
		public string[] Input { get; } = input;
		public static Command Parse(string playerInput) // Will create a command based on the user input or state that the input is invalid
		{
			CommandType type;
			if (string.IsNullOrWhiteSpace(playerInput)) // Handles null or blank inputs
				return new Command(CommandType.Invalid, []);
			string commandType = playerInput.Split(" ")[0].ToUpper();
			string[] inputData = playerInput.Split(" ")[1..]; // Some command types don't require further info (e.g. undo) This will return an empty array in those cases
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
	}
}
