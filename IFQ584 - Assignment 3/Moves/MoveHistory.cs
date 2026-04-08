namespace TicTacToe_Framewrork
{
public class MoveHistory
{
    private Stack<Move> doneMoves;
    private Stack<Move> undoneMoves;

    public MoveHistory()
    {
        doneMoves   = new Stack<Move>();
        undoneMoves = new Stack<Move>();
    }

    public int CurrentMoveIndex => doneMoves.Count;

    // record a new move
    public void DoMove(Move move)
    {
        doneMoves.Push(move);
        undoneMoves.Clear();
    }

    // undo the most recent move; returns the move so the caller can reverse it
    public Move? Undo()
    {
        if (!CanUndo()) return null;
        Move move = doneMoves.Pop();
        undoneMoves.Push(move);
        return move;
    }

    // redo the most recently undone move; returns the move so the caller can re-apply it
    public Move? Redo()
    {
        if (!CanRedo()) return null;
        Move move = undoneMoves.Pop();
        doneMoves.Push(move);
        return move;
    }

    public bool CanUndo() => doneMoves.Count > 0;
    public bool CanRedo() => undoneMoves.Count > 0;

    public IEnumerable<Move> GetDoneMoves() => doneMoves.Reverse();

    public IEnumerable<Move> GetUndoneMoves() => undoneMoves.Reverse();

    // restore history from saved data
    public void Restore(IEnumerable<Move> done, IEnumerable<Move> undone)
    {
        doneMoves.Clear();
        undoneMoves.Clear();
        foreach (var move in done) doneMoves.Push(move);
        foreach (var move in undone) undoneMoves.Push(move);
    }

    public void Clear()
    {
        doneMoves.Clear();
        undoneMoves.Clear();
    }
}
}
