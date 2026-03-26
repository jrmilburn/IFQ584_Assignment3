class GridBoard : IBoard
{
    private int size;
    private Cell[,] cells;

    public GridBoard(int boardSize)
    {
        size = boardSize;
        cells = new Cell[size, size];

        //iterate over cells to create empty cells
        for(int row = 0; row < size; row++)
        {
            for(int col = 0; col < size; col++)
            {
                cells[row, row] = new EmptyCell();
            }
        }
    }

    public bool IsInBounds(int boardIndex, int row, int col){}

    public bool IsEmpty(int boardIndex, int row, int col){}

    public Cell GetCell(int boardIndex, int row, int col){}

    public bool SetCell(int boardIndex, int row, int col, Cell cell){}

    public void ClearCell(int boardIndex, int row, int col){}

    public bool IsFull(int boardIndex){}

    public int GetBoardCount(){}

    public int GetSize(int boardIndex){}

    public void Display(){}




}