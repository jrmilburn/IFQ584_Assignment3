namespace BoardGames
{
	public interface IRules
	{
		bool IsValid(Move move, IBoard board);
		List<Move> LegalMoves(IBoard board, int playerId);
		GameResult Evaluate(IBoard board);
	}
}
