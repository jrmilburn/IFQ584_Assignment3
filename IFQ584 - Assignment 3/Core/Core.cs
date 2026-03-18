using BoardGames.Interfaces;

namespace BoardGames.Core
{
    // Moves
    public class Move
    {
        public int PlayerId      { get; }
        public int X             { get; }
        public int Y             { get; }
        public string ValueOrPiece { get; }
        public int BoardIndex    { get; }

        public Move(int playerId, int x, int y, string valueOrPiece = "X", int boardIndex = 0)
        {
            PlayerId     = playerId;
            X            = x;
            Y            = y;
            ValueOrPiece = valueOrPiece;
            BoardIndex   = boardIndex;
        }

        public override string ToString() =>
            $"Player {PlayerId} → ({X},{Y}) [{ValueOrPiece}] board={BoardIndex}";
    }

    // MoveHistory 
    public class MoveHistory
    {
        private readonly Stack<Move> _undo = new();
        private readonly Stack<Move> _redo = new();

        public int UndoCount => _undo.Count;
        public int RedoCount => _redo.Count;
        public Move? UndoTop  => _undo.TryPeek(out var m) ? m : null;

        public void DoMove(Move m)
        {
            _undo.Push(m);
            _redo.Clear();          // new move clears redo
        }

        public Move? Undo()
        {
            if (_undo.Count == 0) return null;
            var m = _undo.Pop();
            _redo.Push(m);
            return m;
        }

        public Move? Redo()
        {
            if (_redo.Count == 0) return null;
            var m = _redo.Pop();
            _undo.Push(m);
            return m;
        }

        public void ClearRedo() => _redo.Clear();

        public List<Move> UndoMoves() => _undo.ToList();
        public List<Move> RedoMoves() => _redo.ToList();

        public void Restore(List<Move> undoMoves, List<Move> redoMoves)
        {
            _undo.Clear(); _redo.Clear();
            // Stacks push in reverse
            foreach (var m in Enumerable.Reverse(undoMoves)) _undo.Push(m);
            foreach (var m in Enumerable.Reverse(redoMoves))  _redo.Push(m);
        }
    }

    //  GameState (save/load)
    public class GameState
    {
        public string GameTypeId   { get; set; } = "";
        public string Mode         { get; set; } = "";
        public string BoardData    { get; set; } = "";
        public int    CurrentPlayer{ get; set; }
        public List<string> UndoMoves { get; set; } = new();
        public List<string> RedoMoves { get; set; } = new();
    }

    //  Game (abstract)
    public abstract class Game
    {
        protected IBoard  Board   { get; set; } = null!;
        protected IRules  Rules   { get; set; } = null!;
        protected IPlayer[] Players { get; set; } = null!;

        public int CurrentPlayerIndex { get; set; }
        public IPlayer CurrentPlayer  => Players[CurrentPlayerIndex];
        public string Mode            { get; protected set; } = "";
        public abstract string GameTypeId { get; }

        public abstract List<Move> GetLegalMoves();
        public abstract bool ApplyMove(Move move);
        public abstract bool UndoMove(Move move);
        public abstract GameResult CheckResult();
        public abstract void RenderBoard();
        public abstract GameState Serialise();
        public abstract void RestoreFrom(GameState gs);

        public void NextPlayer() =>
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Length;
    }
}
