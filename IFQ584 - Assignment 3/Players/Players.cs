using static System.Console;

namespace BoardGames
{
	public abstract class Player(int id, string name)
	{
        public int ID { get; } = id;
        public string Name { get; } = name;
		public abstract Command GetCommand(Game game);
	}
	public class HumanPlayer(int id, string name) : Player(id, name)
	{
		public override Command GetCommand(Game game)
		{
			Command input = CommandParser.Parse(ReadLine());
			return input;
		}	
	}
	//  ComputerPlayer 
	public class ComputerPlayer(int id) : Player(id, "CPU")
	{
		private readonly Random _rng = new();

		public override Command GetCommand(Game game)
		{
			List<Move> availableMoves = game.GetLegalMoves();
			Move? comMove = FindWinningMove(game, availableMoves);
			if (comMove == null)
			{
				comMove = availableMoves[_rng.Next(availableMoves.Count)];
			}
				Command playedMove = CommandParser.Parse($"move {comMove.X} {comMove.Y} {comMove.ValueOrPiece} {comMove.BoardIndex}");
				return playedMove;
		}  
		private static Move? FindWinningMove(Game game, List<Move> candidates)
		{
			foreach(Move move in candidates)
			{
				game.ApplyMove(move);
				if (game.CheckResult() == GameResult.Win)
				{
					game.UndoMove(move);
					return move;
				}
				else
					game.UndoMove(move);
			}
			return null;
		}
	}
}
