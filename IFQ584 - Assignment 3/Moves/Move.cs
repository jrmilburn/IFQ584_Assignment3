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
        piece = null;
    }

    public Move(int b, int r, int c, PieceType p)
    {
        boardIndex = b;
        row = r;
        col = c;
        piece = p;
        val = null;
    }

    public int BoardIndex => boardIndex;
    public int Row => row;
    public int Col => col;
    public int? Val => val;
    public PieceType? Piece => piece;
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

