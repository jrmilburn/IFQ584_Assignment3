//Contains move, piece and which player made the move
class MoveRecord
{
    private Move move;
    private PieceType piece;
    private int PlayerIndex;
    public MoveRecord(Move m, PieceType p, int index)
    {
        move = m;
        piece = p;
        PlayerIndex = index;
    }
}