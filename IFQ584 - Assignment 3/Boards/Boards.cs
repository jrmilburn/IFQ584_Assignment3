using System;
using System.Collections.Generic;
using System.Linq;
using static System.Console;

// GridBoard – general n×n string grid (Gomoku, Notakto sub-boards)
public class GridBoard : IBoard
{
    public int Size { get; }
    public bool Dead { get; set; }      // used by Notakto to mark an eliminated board

    private string[,] cells;

    public GridBoard(int boardSize)
    {
        Size = boardSize;
        cells = new string[boardSize, boardSize];
        for (int row = 0; row < boardSize; row++)
            for (int col = 0; col < boardSize; col++)
                cells[row, col] = ".";
    }

    private GridBoard(GridBoard src)
    {
        Size = src.Size;
        Dead = src.Dead;
        cells = (string[,])src.cells.Clone();
    }

    public bool IsEmpty(int x, int y) => cells[y, x] == ".";
    public string GetCell(int x, int y) => cells[y, x];
    public void SetCell(int x, int y, string value) => cells[y, x] = value;
    public IBoard Clone() => new GridBoard(this);

    public string[] GetRow(int r, int index = 0)
    {
        string[] row = new string[Size];
        for (int c = 0; c < Size; c++)
            row[c] = cells[r, c];
        return row;
    }

    public string[] GetColumn(int c, int index = 0) // returns an array of the move values for the column at the specified column index
    {
        string[] column = new string[Size];
        for (int r = 0; r < Size; r++)
            column[r] = cells[r, c];
        return column;
    }

    public string[] GetDiagonal(bool leftToRight, int index = 0)
    {
        string[] diagonal = new string[Size];
        if (leftToRight)
            for (int rc = 0; rc < Size; rc++)
                diagonal[rc] = cells[rc, rc];
        else
            for (int rc = 0; rc < Size; rc++)
                diagonal[rc] = cells[rc, Size - 1 - rc];
        return diagonal;
    }

    public (int, int)[] GetEmptyCells(int index = 0)
    {
        List<(int, int)> emptyCells = [];
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (IsEmpty(c, r))
                    emptyCells.Add((c, r));
        return emptyCells.ToArray();
    }

    public bool Contains(string value)
    {
        foreach (string cell in cells)
            if (value == cell)
                return true;
        return false;
    }

    public IBoard GetBoardAtIndex(int i)
    {
        return this;
    }

    public bool IsDead()
    {
        return Dead;
    }

    // Format: "size|r0c0,r0c1,...;r1c0,...|Dead"
    public string Serialise()
    {
        var rows = new List<string>();
        for (int row = 0; row < Size; row++)
        {
            var cols = new List<string>();
            for (int col = 0; col < Size; col++) cols.Add(cells[row, col]);
            rows.Add(string.Join(",", cols));
        }
        return $"{Size}|{string.Join(";", rows)}|{Dead}";
    }

    public static GridBoard Deserialise(string data)
    {
        var parts = data.Split('|');
        int size = int.Parse(parts[0]);
        var board = new GridBoard(size) { Dead = bool.Parse(parts[2]) };
        var rows = parts[1].Split(';');
        for (int row = 0; row < size; row++)
        {
            var cols = rows[row].Split(',');
            for (int col = 0; col < size; col++)
                board.cells[row, col] = cols[col];
        }
        return board;
    }

    public void Render()
    {
        Write("   ");
        for (int col = 0; col < Size; col++) Write($"{col,3}");
        WriteLine();
        for (int row = 0; row < Size; row++)
        {
            Write($"{row,2} ");
            for (int col = 0; col < Size; col++)
                Write($"{cells[row, col],3}");
            WriteLine();
        }
    }

    // True if the given piece appears n-in-a-row in any of the four directions
    //THINK THIS WILL BE CHECKED IN RULES ASWELL
    /*public bool HasNInARow(string piece, int n)
    {
        int[,] dirs = { { 1, 0 }, { 0, 1 }, { 1, 1 }, { 1, -1 } };
        for (int row = 0; row < Size; row++)
        for (int col = 0; col < Size; col++)
        {
            if (cells[row, col] != piece) continue;
            for (int d = 0; d < 4; d++)
            {
                int dr = dirs[d, 0], dc = dirs[d, 1], count = 1;
                for (int k = 1; k < n; k++)
                {
                    int nr = row + dr * k, nc = col + dc * k;
                    if (nr < 0 || nr >= Size || nc < 0 || nc >= Size) break;
                    if (cells[nr, nc] == piece) count++; else break;
                }
                if (count >= n) return true;
            }
        }
        return false;
    }*/

    public bool IsFull()
    {
        for (int row = 0; row < Size; row++)
            for (int col = 0; col < Size; col++)
                if (cells[row, col] == ".") return false;
        return true;
    }

    // Returns all empty cell positions as (x, y) pairs
    public List<(int x, int y)> EmptyCells()
    {
        var list = new List<(int, int)>();
        for (int row = 0; row < Size; row++)
            for (int col = 0; col < Size; col++)
                if (cells[row, col] == ".") list.Add((col, row));
        return list;
    }
}

// MultiBoard – Notakto: three independent n×n GridBoards
// IBoard calls are routed to the sub-board selected by SetRouteIndex
public class MultiBoard : IBoard
{
    public GridBoard[] Boards { get; }
    public int ActiveBoards => Boards.Count(b => !b.Dead);

    private int routeIndex;

    public MultiBoard()
    {
        Boards = new[] { new GridBoard(3), new GridBoard(3), new GridBoard(3) }; //Need to update to be n x n?
    }

    private MultiBoard(MultiBoard src)
    {
        Boards = src.Boards.Select(b => (GridBoard)b.Clone()).ToArray();
        routeIndex = src.routeIndex;
    }

    public void SetRouteIndex(int i) => routeIndex = i;
    public bool IsEmpty(int x, int y) => Boards[routeIndex].IsEmpty(x, y);
    public string GetCell(int x, int y) => Boards[routeIndex].GetCell(x, y);
    public void SetCell(int x, int y, string value) => Boards[routeIndex].SetCell(x, y, value);

    public string[] GetRow(int row, int index = 0)
    {
        return Boards[index].GetRow(row);
    }

    public string[] GetColumn(int col, int index = 0)
    {
        return Boards[index].GetColumn(col);
    }

    public string[] GetDiagonal(bool leftToRight, int index = 0)
    {
        return Boards[index].GetDiagonal(leftToRight);
    }

    public (int, int)[] GetEmptyCells(int index = 0)
    {
        return Boards[index].GetEmptyCells();
    }

    public bool IsFull()
    {
        return Boards.All(board => board.IsFull());
    }

    public bool Contains(string ValueOrPiece)
    {
        return Boards.Any(board => board.Contains(ValueOrPiece));
    }

    public IBoard GetBoardAtIndex(int i)
    {
        return Boards[i];
    }

    public bool IsDead()
    {
        return false;
    }

    public IBoard Clone() => new MultiBoard(this);

    // Sub-boards separated by '~'
    public string Serialise() => string.Join("~", Boards.Select(b => b.Serialise()));

    public static MultiBoard Deserialise(string data)
    {
        var mb = new MultiBoard();
        var parts = data.Split('~');
        for (int i = 0; i < 3; i++)
            mb.Boards[i] = GridBoard.Deserialise(parts[i]);
        return mb;
    }

    public void Render()
    {
        for (int i = 0; i < 3; i++)
        {
            string status = Boards[i].Dead ? "[DEAD]" : "[LIVE]";
            WriteLine($"  Board {i} {status}");
            Boards[i].Render();
            WriteLine();
        }
    }
}