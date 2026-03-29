public class Move
{
    private int playerId;
    private int x;
    private int y;
    private string valueOrPiece;
    private int boardIndex;

    public Move(int playerId, int x, int y, string valueOrPiece, int boardIndex = 0)
    {
        this.playerId     = playerId;
        this.x            = x;
        this.y            = y;
        this.valueOrPiece = valueOrPiece;
        this.boardIndex   = boardIndex;
    }

    public int PlayerId => playerId;
    public int X => x;
    public int Y => y;
    public string ValueOrPiece => valueOrPiece;
    public int BoardIndex => boardIndex;

    public override string ToString() =>
        $"Player {playerId}: ({x},{y}) = {valueOrPiece}" +
        (boardIndex != 0 ? $" [board {boardIndex}]" : "");
}

public enum PieceType
{
    None = 0,
    Odds = 1,
    Evens = 2,
    X = 3,
    O = 4,
    Neutral = 5
}
