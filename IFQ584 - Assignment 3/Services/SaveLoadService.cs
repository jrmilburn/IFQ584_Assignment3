using System.Text.Json;

namespace BoardGames
{
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

			var json = File.ReadAllText(path);
			var state = JsonSerializer.Deserialize<GameState>(json);
			if (state == null) return null;

			var mode = Enum.Parse<GameMode>(state.Mode);
			var game = GameFactory.Create(state.GameTypeId, mode);
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
}
