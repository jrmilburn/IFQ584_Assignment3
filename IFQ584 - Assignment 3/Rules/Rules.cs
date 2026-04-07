namespace BoardGames
{
	public enum GameResult { NotFinished, Win, Loss, Tie } // maintain the state of the game and allows GameController.Run() to determine whe a game is over

	public class GomokuRules() : IRules
	{
        private const int WINLENGTH = 5; // The number of pieces need to be in a line to win the game.
		private const int BOARDSIZE = 15;
		public bool IsValid(Move proposedMove, IBoard board, int _) 
		{
			return board.InBounds(proposedMove.X, proposedMove.Y) && board.IsEmpty(proposedMove.X, proposedMove.Y); 
		}
		public Move[] GetAvailableMoves(IBoard board, int playerId) // Determines all the available cells on the board where a move is available to be placed and provides a complete list of moves for the speciific player (for AI)
		{
			List<Move> availableMoves = [];
			string validPiece;
			if (playerId == 1)
				validPiece = "X";
			else
				validPiece = "O";
			foreach ((int x, int y) in board.GetEmptyCells())
				availableMoves.Add(new(playerId, x, y, validPiece, 0));
			return availableMoves.ToArray(); // set to array to prevent external modification of the list
		}
		private static bool HasWinningLine(IBoard board) // Itereates through every line to determine if the line wins
		{
			for (int i = 0; i < BOARDSIZE; i++)
			{
				if (IsWinningLine(board.GetRow(i))) return true;
				if (IsWinningLine(board.GetColumn(i))) return true;
			}
			if (IsWinningLine(board.GetDiagonal(true))) return true;
			if (IsWinningLine(board.GetDiagonal(false))) return true;
			return false;
        }
		private static bool IsWinningLine(string[] line) // Counts all the peices in a row on each line and returns true if the count equal WINLENGTH
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
				if (count >= WINLENGTH)
					return true;
				prevPiece = currentPiece;
			}
			return false;
		}
		public GameResult Evaluate(IBoard board) // Proivides the current status of the board
		{
			if (HasWinningLine(board)) return GameResult.Win;
			if (board.IsFull()) return GameResult.Tie;
			return GameResult.NotFinished;
		}
	}

	public class NumericalTTTRules : IRules
	{
		private readonly int BoardSize;
		private readonly int NumCells;
		private readonly int WinningScore;
		private readonly Dictionary<int, List<string>> PlayerNumbers;
		public NumericalTTTRules(int boardSize)
		{
			BoardSize = boardSize;
			NumCells = boardSize * boardSize;
			WinningScore = (BoardSize * ((BoardSize * BoardSize) + 1)) / 2;
			PlayerNumbers = new() { { 1, [] }, { 2, [] } }; // Dictioinary is used to allow for easy lookup of the valid moves available to the player.
			for (int i = 1; i < NumCells + 1; i++)
			{
				if (int.IsOddInteger(i))
					PlayerNumbers[1].Add(i.ToString());
				else
					PlayerNumbers[2].Add(i.ToString());
			}
		}
		public bool IsValid(Move proposedMove, IBoard board, int playerId) 
		{
			return board.InBounds(proposedMove.X, proposedMove.Y) &&
                !board.Contains(proposedMove.ValueOrPiece) && // Checks that the proposed move does not contain a played piece already played.
                board.IsEmpty(proposedMove.X, proposedMove.Y) &&
                PlayerNumbers[playerId].Contains(proposedMove.ValueOrPiece);
		}
		public Move[] GetAvailableMoves(IBoard board, int playerId) // Will creat an array of all possible moves for the AI including all potential piece combinations available
		{
			List<Move> availableMoves = [];
			String[] availablePieces = GetAvailablePieces(board, playerId);
			foreach ((int x, int y) in board.GetEmptyCells())
			{
				foreach (string number in availablePieces)
					availableMoves.Add(new(playerId, x, y, number.ToString(), 0));
			}
			return availableMoves.ToArray();
		}
		private string[] GetAvailablePieces(IBoard board, int playerId)
		{
			List<string> playableNum = [];
			foreach (string number in PlayerNumbers[playerId])
				if (!board.Contains(number))
					playableNum.Add(number);
			return playableNum.ToArray();
		}
		private bool HasWinningLine(IBoard board) 
		{
            for (int i = 0; i < BoardSize; i++)
            {
                if (IsWinningLine(board.GetRow(i))) return true;
                if (IsWinningLine(board.GetColumn(i))) return true;
            }
            if (IsWinningLine(board.GetDiagonal(true))) return true;
            if (IsWinningLine(board.GetDiagonal(false))) return true;
            return false;
        }
        private bool IsWinningLine(string[] line) // A winning line is one that is full and is equal to the WinnignScore.
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
	public class NotaktoRules : IRules
	{
		private const int BOARDCOUNT = 3;
		private const int BOARDSIZE = 3;
		private const string VALIDPIECE = "X";
		private readonly Random r = new();
		public bool IsValid(Move proposedMove, IBoard board, int _) // There is only one piece type so Player ID is not needed for peice validation.
		{
			return proposedMove.BoardIndex >= 0 && proposedMove.BoardIndex < BOARDCOUNT &&
                board.InBounds(proposedMove.X, proposedMove.Y) &&
                board.GetBoardAtIndex(proposedMove.BoardIndex).IsEmpty(proposedMove.X, proposedMove.Y) &&
				!board.GetBoardAtIndex(proposedMove.BoardIndex).IsDead();
		}
		public Move[] GetAvailableMoves(IBoard board, int playerId)
		{
			List<Move> availableMoves = [];
			for (int i = 0; i < BOARDCOUNT; i++)
			{
				IBoard currentBoard = board.GetBoardAtIndex(i);
				if (!currentBoard.IsDead())
					foreach ((int x, int y) in currentBoard.GetEmptyCells())
						availableMoves.Add(new(playerId, x, y, VALIDPIECE, i));
			}
			Move[] shuffledMoves = availableMoves.ToArray();
			r.Shuffle(shuffledMoves);
			return shuffledMoves;
		}
        private static bool IsIncompleteLine(string[] line)
        {
            foreach (string piece in line)
                if (piece == ".")
                    return true;
            return false;
        }
        private static bool HasLosingLine(IBoard board) 
		{
			for (int i = 0; i < BOARDSIZE; i++)
			{
				if (!IsIncompleteLine(board.GetRow(i))) return true;
				if (!IsIncompleteLine(board.GetColumn(i))) return true;
			}
			if (!IsIncompleteLine(board.GetDiagonal(true))) return true;
			if (!IsIncompleteLine(board.GetDiagonal(false))) return true;
			return false;
		}
		public GameResult Evaluate(IBoard board) // Used to update boardstate and return current game status
		{
			for (int b = 0; b < BOARDCOUNT; b++)
			{
				IBoard currentBoard = board.GetBoardAtIndex(b);
				if (HasLosingLine(currentBoard))
					currentBoard.Dead = true;
			}
            if (board.IsDead()) return GameResult.Loss;
            return GameResult.NotFinished;
        }
	}
}