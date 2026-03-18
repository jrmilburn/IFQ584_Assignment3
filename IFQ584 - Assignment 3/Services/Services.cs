using BoardGames.Core;
using BoardGames.Games;
using System.Text.Json;

namespace BoardGames.Services
{
    //  SaveLoadService 
    public class SaveLoadService
    {
        public void Save(string path, GameState state, MoveHistory history)
        {
            state.UndoMoves = history.UndoMoves()
                .Select(m => $"{m.PlayerId},{m.X},{m.Y},{m.ValueOrPiece},{m.BoardIndex}")
                .ToList();
            state.RedoMoves = history.RedoMoves()
                .Select(m => $"{m.PlayerId},{m.X},{m.Y},{m.ValueOrPiece},{m.BoardIndex}")
                .ToList();

            var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, json);
            Console.WriteLine($"  [Save] Game saved to {path}");
        }

        public (Game game, MoveHistory history)? Load(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine($"  [Load] File not found: {path}");
                return null;
            }

            var json  = File.ReadAllText(path);
            var state = JsonSerializer.Deserialize<GameState>(json);
            if (state == null) return null;

            var mode    = Enum.Parse<GameMode>(state.Mode);
            var game    = GameFactory.Create(state.GameTypeId, mode);
            game.RestoreFrom(state);

            var history = new MoveHistory();
            history.Restore(
                state.UndoMoves.Select(ParseMove).ToList(),
                state.RedoMoves.Select(ParseMove).ToList()
            );

            Console.WriteLine($"  [Load] Restored {state.GameTypeId} ({state.Mode})");
            return (game, history);
        }

        private static Move ParseMove(string s)
        {
            var p = s.Split(',');
            return new Move(int.Parse(p[0]), int.Parse(p[1]), int.Parse(p[2]), p[3], int.Parse(p[4]));
        }
    }

    //  Command 
    public enum CommandType { Move, Undo, Redo, Save, Load, Help, Quit, Invalid }

    public class Command
    {
        public CommandType Type   { get; }
        public string[]    Args   { get; }

        public Command(CommandType type, string[] args) { Type = type; Args = args; }
    }

    //  CommandParser 
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
                _     => new Command(CommandType.Move, parts)
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
