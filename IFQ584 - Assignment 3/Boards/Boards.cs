using System;
using static System.Console;

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

    public bool IsInBounds(int boardIndex, int row, int col)
    {
        //board position greater than 0 and less than board size for row and col
        return boardIndex == 0 && row >= 0 && row < size && col >= 0 && col < size;
    }

    public bool IsEmpty(int boardIndex, int row, int col)
    {
        //check in bounds
        if(!IsInBounds(boardIndex, row, col))
        {
            return false;
        }

        return cells[row, col].IsEmpty();
    }

    public Cell GetCell(int boardIndex, int row, int col)
    {
        if(!IsInBounds(boardIndex, row, col))
        {
            throw new ArgumentException("Cell is out of bounds.");
        }

        return cells[row, col];
    }

    public bool SetCell(int boardIndex, int row, int col, Cell cell){}

    public void ClearCell(int boardIndex, int row, int col){}

    public bool IsFull(int boardIndex){}

    public int GetBoardCount()
    {
        return 1;
    }

    public int GetSize(int boardIndex){}

    public void Display()
    {
        WriteLine();

        //Board display
        for(int row = 0; row < size; row++)
        {
            for(int col = 0; col < size; col++)
            {
                WriteLine(cells[row, col].GetDisplayText());
            }
            WriteLine();
        }
        WriteLine();

    }




}