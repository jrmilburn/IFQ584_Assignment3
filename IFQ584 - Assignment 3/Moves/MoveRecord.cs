//Contains move, piece and which player made the move
class MoveRecord
{
    private Move move;
    private PieceType piece;
    private int playerIndex;
    public MoveRecord(Move m, PieceType p, int index)
    {
        move = m;
        piece = p;
        playerIndex = index;
    }

    public Move Move => move;
    public PieceType Piece => piece;
    public int PlayerIndex => playerIndex;
}