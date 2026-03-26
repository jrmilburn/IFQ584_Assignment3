interface IBoard
{
    bool IsInBounds(int boardIndex, int row, int col);
    bool IsEmpty(int boardIndex, int row, int col);
    Cell GetCell(int boardIndex, int row, int col);
    bool SetCell(int boardIndex, int row, int col, Cell cell);
    void ClearCell(int boardIndex, int row, int col);
    bool IsFull(int boardIndex);
    int GetBoardCount();
    int GetSize(int boardIndex);
    void Display();
    string Serialise();
}