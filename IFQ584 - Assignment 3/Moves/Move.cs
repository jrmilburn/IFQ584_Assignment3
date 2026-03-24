//Contains move data exclusively
class Move
{
    private int boardIndex;
    private int row;
    private int col;
    private int? val;
    private PieceType? piece;


    public Move(int b, int r, int c, int v)
    {
        boardIndex = b;
        row = r;
        col = c;
        val = v;
    }

    public Move(int b, int r, int c, PieceType p)
    {
        boardIndex = b;
        row = r;
        col = c;
        piece = p;
    }
}

enum PieceType
{
    None = 0,
    Odds = 1,
    Evens = 2,
    X = 3,
    O = 4,
    Neutral = 5

}

