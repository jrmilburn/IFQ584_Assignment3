namespace BoardGames
{
	public abstract class Player(int id, string name)
	{
        public int ID { get; } = id;
        public string Name { get; } = name;
		public abstract Move GetMove(Game game);
	}
	public class HumanPlayer(int id, string name) : Player(id, name)
	{
		public override Move GetMove(Game game)
		{
			// collect a raw line
			//  delegate validation back through game.GetLegalMoves / IRules.
			while (true)
			{
				Console.Write($"  {Name} > ");
				var input = Console.ReadLine()?.Trim() ?? "";


				// embed the raw input in a special "human input" move using
				// x=y=-1 as a marker; GameController handles it.
				return new Move(ID, -1, -1, input);
			}
		}
	}
	//  ComputerPlayer 
	public class ComputerPlayer(int id, Random? rng = null) : Player(id, "CPU")
	{
		public int Id { get; } = id;
		private readonly Random _rng = rng ?? new Random();

		public override Move GetMove(Game game)
		{
			var legal = game.GetLegalMoves();
			if (legal.Count == 0) throw new InvalidOperationException("No legal moves.");

			// Try immediate win
			var winning = FindImmediateWin(game, legal);
			if (winning != null)
			{
				Console.WriteLine("  [CPU] Found winning move!");
				return winning;
			}

			// Random fallback
			var chosen = legal[_rng.Next(legal.Count)];
			Console.WriteLine($"  [CPU] Plays ({chosen.X},{chosen.Y})");
			return chosen;
		}

		private Move? FindImmediateWin(Game game, List<Move> candidates)
		{
			foreach (var m in candidates)
			{
				// Use game.ApplyMove / CheckResult / UndoMove to test
				game.ApplyMove(m);
				var result = game.CheckResult();
				game.UndoMove(m);
				if (result == GameResult.Win || result == GameResult.Loss)
					return m;
			}
			return null;
		}
	}
}
