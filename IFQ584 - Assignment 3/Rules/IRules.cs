namespace TicTacToe_Framework
{
	public interface IRules
	{
        public bool IsValid(Move proposedMove, IBoard board, int playerId);
        public Move[] GetAvailableMoves(IBoard board, int playerId);
		public GameResult Evaluate(IBoard board);
	}
}