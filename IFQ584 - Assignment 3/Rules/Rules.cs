namespace BoardGames
{
	public enum GameResult { NotFinished, Win, Loss, Tie }
	//  GomokuRules 
	public class GomokuRules(int winLength) : IRules
	{
		public int WINLENGTH = winLength;
		public const int BOARDSIZE = 15;
		public bool IsValid(Move proposedMove, IBoard board, int playerID)
		{
			return board.IsEmpty(proposedMove.X, proposedMove.Y) &&
				proposedMove.X < BOARDSIZE &&
				proposedMove.Y < BOARDSIZE &&
				proposedMove.X >= 0 &&
				proposedMove.Y >= 0;
		}
		public List<Move> GetAvailableMoves(IBoard board, int playerId)
		{
			List<Move> availableMoves = [];
			string validPiece;
			if (playerId == 1)
				validPiece = "X";
			else
				validPiece = "O";
			foreach ((int x, int y) in board.GetEmptyCells())
				availableMoves.Add(new(playerId, x, y, validPiece, 0));
			return availableMoves;
		}
		public List<string> GetAvailablePieces(IBoard board, int playerId)
		{
			return ["O"];
		}
		public bool HasWinningLine(IBoard board) // itereates through every possible line to determine if the line wins
		{
			for (int i = 0; i < BOARDSIZE; i++)
				if (IsWinningLine(board.GetRow(i)) || IsWinningLine(board.GetColumn(i)))
					return true;
			return (IsWinningLine(board.GetDiagonal(true)) || IsWinningLine(board.GetDiagonal(false)));
		}
		public bool IsWinningLine(string[] line)
		{
			int count = 0;
			string prevPiece = line[0];
			string currentPiece;
			foreach (string piece in line)
			{
				currentPiece = piece;
				if (currentPiece == prevPiece && currentPiece != ".")
					count++;
				else
					count = 0;
				if (count >= 5)
					return true;
				prevPiece = currentPiece;
			}
			return false;
		}
		public GameResult Evaluate(IBoard board)
		{
			if (HasWinningLine(board)) return GameResult.Win;
			if (board.IsFull()) return GameResult.Tie;
			return GameResult.NotFinished;
		}
	}
	//  NumericalTTTRules 
	// Player 1 uses odd numbers (1,3,5,7,9), Player 2 uses even (2,4,6,8).
	// Win = any row/col/diag sums to 15.
	public class NumericalTTTRules : IRules
	{
		private readonly int BoardSize;
		private readonly int NumCells;
		private int WinningScore;
		private readonly Dictionary<int, List<string>> PlayerNumbers;
		public NumericalTTTRules(int boardSize)
		{
			BoardSize = boardSize;
			NumCells = boardSize * boardSize;
			WinningScore = (BoardSize * ((BoardSize * BoardSize) + 1)) / 2;
			PlayerNumbers = new() { { 1, [] }, { 2, [] } };
			for (int i = 1; i < NumCells + 1; i++)
			{
				if (int.IsOddInteger(i))
					PlayerNumbers[1].Add(i.ToString());
				else
					PlayerNumbers[2].Add(i.ToString());
			}
		}
		public bool IsValid(Move proposedMove, IBoard board, int playerID)
		{
			return board.IsEmpty(proposedMove.X, proposedMove.Y) &&
				PlayerNumbers[playerID].Contains(proposedMove.ValueOrPiece) &&
				proposedMove.X < BoardSize &&
				proposedMove.Y < BoardSize &&
				proposedMove.X >= 0 &&
				proposedMove.Y >= 0 &&
				!board.Contains(proposedMove.ValueOrPiece);
		}
		public List<Move> GetAvailableMoves(IBoard board, int playerId)
		{
			List<Move> availableMoves = [];
			List<String> availablePieces = GetAvailablePieces(board, playerId);
			foreach ((int x, int y) in board.GetEmptyCells())
			{
				foreach (string number in availablePieces)
					availableMoves.Add(new(playerId, x, y, number.ToString(), 0));
			}
			return availableMoves;
		}
		public List<string> GetAvailablePieces(IBoard board, int playerId)
		{
			List<string> playableNum = [];
			foreach (string number in PlayerNumbers[playerId])
				if (!board.Contains(number))
					playableNum.Add(number);
			return playableNum;
		}
		public bool HasWinningLine(IBoard board) // itereates through every possible line to determine if the line wins
		{
			for (int i = 0; i < BoardSize; i++)
				if (IsWinningLine(board.GetRow(i)) || IsWinningLine(board.GetColumn(i)))
					return true;
			return (IsWinningLine(board.GetDiagonal(true)) || IsWinningLine(board.GetDiagonal(false)));
		}
		public bool IsWinningLine(string[] line)
		{
			return !line.Contains(".") && line.Sum(int.Parse) == WinningScore;
		}
		public GameResult Evaluate(IBoard board)
		{
			if (HasWinningLine(board)) return GameResult.Win;
			if (board.IsFull()) return GameResult.Tie;
			return GameResult.NotFinished;
		}
	}
	//  NotaktoRules 
	// last to complete a three-in-a-row on ALL boards loses.
	public class NotaktoRules : IRules
	{
		public int LineLength { get; } = 3;
		public int BoardCount { get; } = 3;
		private readonly int BoardSize = 3;
		private const string VALIDPIECE = "X";
		private readonly Random r = new();
		public bool IsValid(Move proposedMove, IBoard board, int playerID)
		{
			return proposedMove.BoardIndex >= 0 &&
				proposedMove.BoardIndex < BoardCount &&
				proposedMove.X < BoardSize &&
				proposedMove.Y < BoardSize &&
				proposedMove.X >= 0 &&
				proposedMove.Y >= 0 &&
				board.GetBoardAtIndex(proposedMove.BoardIndex).IsEmpty(proposedMove.X, proposedMove.Y) &&
				!board.GetBoardAtIndex(proposedMove.BoardIndex).IsDead();
		}
		public List<Move> GetAvailableMoves(IBoard board, int playerId)
		{
			List<Move> availableMoves = [];
			for (int i = 0; i < BoardCount; i++)
			{
				IBoard currentBoard = board.GetBoardAtIndex(i);
				if (!currentBoard.IsDead())
					foreach ((int x, int y) in currentBoard.GetEmptyCells())
						availableMoves.Add(new(playerId, x, y, VALIDPIECE, i));
			}
			Move[] shuffledMoves = availableMoves.ToArray();
			r.Shuffle(shuffledMoves);
			return shuffledMoves.ToList();
		}
		public List<string> GetAvailablePieces(IBoard board, int playerId)
		{
			return [VALIDPIECE];
		}
		public bool HasWinningLine(IBoard board) // itereates through every possible line to determine if the line wins
		{
			bool deadBoard = false;
			for (int b = 0; b < BoardCount; b++)
			{
				IBoard currentBoard = board.GetBoardAtIndex(b);
				if (!currentBoard.IsDead())
				{
					for (int i = 0; i < LineLength; i++)
						if (IsWinningLine(currentBoard.GetRow(i)) && IsWinningLine(currentBoard.GetColumn(i)))
							deadBoard = true;
					if (IsWinningLine(board.GetDiagonal(true)) && IsWinningLine(board.GetDiagonal(false)))
						deadBoard = true;
				}
			}
			return deadBoard;
		}
		public bool HasLosingLine(IBoard board) // itereates through every possible line to determine if the line wins
		{
			if (!board.IsDead())
			{
				for (int i = 0; i < BoardCount; i++)
				{
					if (!IsWinningLine(board.GetRow(i))) return true;
					if (!IsWinningLine(board.GetColumn(i))) return true;
					if (!IsWinningLine(board.GetDiagonal(true))) return true;
					if (!IsWinningLine(board.GetDiagonal(false))) return true;
				}
			}
			return false;
		}
		public bool IsWinningLine(string[] line)
		{
			foreach (string piece in line)
				if (piece == ".")
				{
					return true;
				}
			return false;
		}
		public GameResult Evaluate(IBoard board)
		{
			int deadBoardCount = 0;
			for (int b = 0; b < BoardCount; b++)
			{
				IBoard currentBoard = board.GetBoardAtIndex(b);
				if (HasLosingLine(currentBoard))
					currentBoard.Dead = true;
				if (currentBoard.IsDead())
					deadBoardCount++;
			}
			if (deadBoardCount == BoardCount) return GameResult.Loss;
			return GameResult.NotFinished;
		}
	}
}
