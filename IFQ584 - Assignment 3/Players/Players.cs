namespace BoardGames
{
	public abstract class Player(int id, string name) // Responsible for handling human and computer command inputs
	{
        public int ID { get; } = id; // Player ID for handling move history
        public string Name { get; } = name; // Player name for UI turn signaling
		public abstract Command GetCommand(Game game);
	}
	public class HumanPlayer(int id, string name) : Player(id, name)
	{
		public override Command GetCommand(Game _) // HumanPlayer doesn't require game, however is still passed in to maintain polymorphism
		{
			Command input = Command.Parse(Console.ReadLine()); // If user inputs a null value, the command parser will create an invalid command
			return input;
		}	
	}
	public class ComputerPlayer(int id) : Player(id, "CPU")
	{
		private readonly Random _rng = new(); // To allow the CPU to pick a valid random move

		public override Command GetCommand(Game game)
		{
			List<Move> availableMoves = game.GetLegalMoves(); // A list of moves available to the computer player
			Move? comMove = FindWinningMove(game, availableMoves); // comMove is allowed to be null if there is no winning move available. Will be replaced by a random move in the next line
			if (comMove == null)
				comMove = availableMoves[_rng.Next(availableMoves.Count)]; 
			Command playedMove = Command.Parse($"move {comMove.X} {comMove.Y} {comMove.ValueOrPiece} {comMove.BoardIndex}"); 
			return playedMove;
		}  
		private static Move? FindWinningMove(Game game, List<Move> availableMoves) // Will provide null value if no winning move available game. This is handled in GetCommand
		{
			foreach(Move move in availableMoves)
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
