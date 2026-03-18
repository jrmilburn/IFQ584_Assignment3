using BoardGames.Interfaces;
using BoardGames.Core;
using BoardGames.Boards;

namespace BoardGames.Rules
{
    //  GomokuRules 
    public class GomokuRules : IRules
    {
        public int WinLength { get; }
        public GomokuRules(int winLength = 5) { WinLength = winLength; }

        public bool IsValid(Move move, IBoard board) =>
            board.IsEmpty(move.X, move.Y);

        public List<Move> LegalMoves(IBoard board, int playerId)
        {
            var gb   = (GridBoard)board;
            string piece = playerId == 1 ? "X" : "O";
            return gb.EmptyCells()
                     .Select(c => new Move(playerId, c.x, c.y, piece))
                     .ToList();
        }

        public GameResult Evaluate(IBoard board)
        {
            var gb = (GridBoard)board;
            if (gb.HasNInARow("X", WinLength)) return GameResult.Win;
            if (gb.HasNInARow("O", WinLength)) return GameResult.Loss;
            if (gb.IsFull()) return GameResult.Tie;
            return GameResult.NotFinished;
        }

        public bool IsWinningMove(Move move, IBoard board)
        {
            var clone = board.Clone();
            clone.SetCell(move.X, move.Y, move.ValueOrPiece);
            return Evaluate(clone) != GameResult.NotFinished;
        }
    }

    //  NumericalTTTRules 
    // Player 1 uses odd numbers (1,3,5,7,9), Player 2 uses even (2,4,6,8).
    // Win = any row/col/diag sums to 15.
    public class NumericalTTTRules : IRules
    {
        private static readonly int[] OddNums  = { 1, 3, 5, 7, 9 };
        private static readonly int[] EvenNums = { 2, 4, 6, 8 };

        public bool IsValid(Move move, IBoard board)
        {
            if (!board.IsEmpty(move.X, move.Y)) return false;
            if (!int.TryParse(move.ValueOrPiece, out int v)) return false;
            var nb    = (NumericBoard)board;
            var used  = nb.UsedNumbers();
            if (used.Contains(v)) return false;
            bool isOdd = v % 2 == 1;
            return move.PlayerId == 1 ? isOdd : !isOdd;
        }

        public List<Move> LegalMoves(IBoard board, int playerId)
        {
            var nb     = (NumericBoard)board;
            var used   = nb.UsedNumbers();
            int[] pool = playerId == 1 ? OddNums : EvenNums;
            var moves  = new List<Move>();
            for (int r = 0; r < 3; r++)
            for (int c = 0; c < 3; c++)
            {
                if (!nb.IsEmpty(c, r)) continue;
                foreach (int n in pool)
                    if (!used.Contains(n))
                        moves.Add(new Move(playerId, c, r, n.ToString()));
            }
            return moves;
        }

        public GameResult Evaluate(IBoard board)
        {
            var nb = (NumericBoard)board;
            if (nb.AnyLineSums15()) return GameResult.Win; // last move wins
            if (nb.IsFull()) return GameResult.Tie;
            return GameResult.NotFinished;
        }

        public bool IsWinningMove(Move move, IBoard board)
        {
            var clone = board.Clone();
            clone.SetCell(move.X, move.Y, move.ValueOrPiece);
            return Evaluate(clone) != GameResult.NotFinished;
        }
    }

    //  NotaktoRules 
    // last to complete a three-in-a-row on ALL boards loses.
    public class NotaktoRules : IRules
    {
        public int LineLength  { get; } = 3;
        public int BoardCount  { get; } = 3;

        public bool IsValid(Move move, IBoard board)
        {
            var mb = (MultiBoard)board;
            var gb = mb.Boards[move.BoardIndex];
            return !gb.Dead && gb.IsEmpty(move.X, move.Y);
        }

        public List<Move> LegalMoves(IBoard board, int playerId)
        {
            var mb    = (MultiBoard)board;
            var moves = new List<Move>();
            for (int bi = 0; bi < mb.Boards.Length; bi++)
            {
                if (mb.Boards[bi].Dead) continue;
                foreach (var (x, y) in mb.Boards[bi].EmptyCells())
                    moves.Add(new Move(playerId, x, y, "X", bi));
            }
            return moves;
        }

        public GameResult Evaluate(IBoard board)
        {
            var mb = (MultiBoard)board;
            // Kill boards that have three-in-a-row
            foreach (var gb in mb.Boards)
                if (!gb.Dead && gb.HasNInARow("X", 3)) gb.Dead = true;

            if (mb.ActiveBoards == 0) return GameResult.Loss; // last mover loses 
            return GameResult.NotFinished;
        }
    }
}
