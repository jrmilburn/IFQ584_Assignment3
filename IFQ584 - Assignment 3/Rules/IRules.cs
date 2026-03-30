namespace BoardGames
{
	public interface IRules
	{
		public bool IsValid(Move proposedMove, IBoard board, int playerID);
		public List<Move> GetAvailableMoves(IBoard board, int playerId);
		public List<string> GetAvailablePieces(IBoard board, int playerId);
		public bool HasWinningLine(IBoard board); // itereates through every possible line to determine if the line wins
		public bool IsWinningLine(string[] line);
		public GameResult Evaluate(IBoard board);
	}
}