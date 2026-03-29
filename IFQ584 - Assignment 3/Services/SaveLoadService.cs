using System.Text.Json;
using BoardGames;

// Responsible for persisting and restoring a game session to disk as a JSON file.
// Handles both the board/player state (via GameState) and the full move history
//so that undo and redo work correctly after a load.
class SaveLoadService
{
    // serialises each move as a comma-separated string before writing to JSON
    public void Save(string path, GameState state, MoveHistory history)
    {
        // flatten the undo and redo stacks into the GameState before serialising
        state.UndoMoves = history.GetDoneMoves().Select(m => $"{m.PlayerId},{m.X},{m.Y},{m.ValueOrPiece},{m.BoardIndex}").ToList();
        state.RedoMoves = history.GetUndoneMoves().Select(m => $"{m.PlayerId},{m.X},{m.Y},{m.ValueOrPiece},{m.BoardIndex}").ToList();
        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
        Console.WriteLine($"  [Save] Game saved to {path}");
    }

    // Reconstruct game from JSON file
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

        // recreate the correct game type and restore its board and current player
        var mode = Enum.Parse<GameMode>(state.Mode);
        var game = GameFactory.Create(state.GameTypeId, mode);
        game.RestoreFrom(state);

        // rebuild the move history from the saved move strings
        var history = new MoveHistory();
        history.Restore(
            state.UndoMoves.Select(ParseMove).ToList(),
            state.RedoMoves.Select(ParseMove).ToList()
        );
        Console.WriteLine($"  [Load] Restored {state.GameTypeId} ({state.Mode})");
        return (game, history);
    }

    // parses a move from its comma-separated string representation
    private Move ParseMove(string s)
    {
        var p = s.Split(',');
        return new Move(
            int.Parse(p[0]),    // playerId
            int.Parse(p[1]),    // x
            int.Parse(p[2]),    // y
            p[3],               // valueOrPiece
            int.Parse(p[4])     // boardIndex
        );
    }
}
