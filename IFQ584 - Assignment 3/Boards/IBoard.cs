namespace TicTacToe_Framework
{
    public interface IBoard
    {
        bool Dead { get; set; }
        bool IsEmpty(int x, int y);
        void SetCell(int x, int y, string value);
        (int, int)[] GetEmptyCells(int index = 0);
        IBoard GetBoardAtIndex(int index = 0);
        bool IsDead();
        string[] GetRow(int row, int index = 0);
        string[] GetColumn(int col, int index = 0);
        string[] GetDiagonal(bool leftToRight, int index = 0);
        bool IsFull();
        bool Contains(string ValueOrPiece);
        bool InBounds(int x, int y);
        string Serialise();
        void Render();
    }
}
