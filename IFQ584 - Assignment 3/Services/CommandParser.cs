namespace BoardGames
{
	public class CommandParser
	{
		public Command Parse(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
				return new Command(CommandType.Invalid, Array.Empty<string>());

			var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
			return parts[0].ToUpper() switch
			{
				"UNDO" => new Command(CommandType.Undo, parts),
				"REDO" => new Command(CommandType.Redo, parts),
				"SAVE" => new Command(CommandType.Save, parts),
				"LOAD" => new Command(CommandType.Load, parts),
				"HELP" => new Command(CommandType.Help, parts),
				"QUIT" or "EXIT" => new Command(CommandType.Quit, parts),
				_ => new Command(CommandType.Move, parts)
			};
		}

		public void ShowHelp()
		{
			Console.WriteLine();
			Console.WriteLine("  ┌─────────────────────────────────────────────────────┐");
			Console.WriteLine("  │                     COMMANDS                        │");
			Console.WriteLine("  ├─────────────────────────────────────────────────────┤");
			Console.WriteLine("  │ Gomoku/Notakto move:  MOVE <col> <row> [boardIndex] │");
			Console.WriteLine("  │ Numerical TTT move:   MOVE <col> <row> <number>     │");
			Console.WriteLine("  │ Undo last move:       UNDO                          │");
			Console.WriteLine("  │ Redo undone move:     REDO                          │");
			Console.WriteLine("  │ Save game:            SAVE <filename>               │");
			Console.WriteLine("  │ Load game:            LOAD <filename>               │");
			Console.WriteLine("  │ Show this help:       HELP                          │");
			Console.WriteLine("  │ Quit game:            QUIT                          │");
			Console.WriteLine("  └─────────────────────────────────────────────────────┘");
			Console.WriteLine();
		}
	}
}
