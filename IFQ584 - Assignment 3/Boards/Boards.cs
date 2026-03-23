namespace BoardGames
{
    // GridBoard 
    public class GridBoard : IBoard
    {
        public int Size { get; }
        public bool Dead { get; set; }          // used for Notakto
        private readonly string[,] _cells;

        public GridBoard(int size)
        {
            Size   = size;
            _cells = new string[size, size];
            for (int r = 0; r < size; r++)
                for (int c = 0; c < size; c++)
                    _cells[r, c] = ".";
        }

        private GridBoard(GridBoard src)
        {
            Size   = src.Size;
            Dead   = src.Dead;
            _cells = (string[,])src._cells.Clone();
        }

        public bool   IsEmpty(int x, int y) => _cells[y, x] == ".";
        public string GetCell(int x, int y)  => _cells[y, x];
        public void   SetCell(int x, int y, string v) => _cells[y, x] = v;
        public IBoard Clone() => new GridBoard(this);

        public string Serialise()
        {
            var rows = new List<string>();
            for (int r = 0; r < Size; r++)
            {
                var cols = new List<string>();
                for (int c = 0; c < Size; c++) cols.Add(_cells[r, c]);
                rows.Add(string.Join(",", cols));
            }
            return $"{Size}|{string.Join(";", rows)}|{Dead}";
        }

        public static GridBoard Deserialise(string data)
        {
            var parts = data.Split('|');
            int size  = int.Parse(parts[0]);
            var board = new GridBoard(size) { Dead = bool.Parse(parts[2]) };
            var rows  = parts[1].Split(';');
            for (int r = 0; r < size; r++)
            {
                var cols = rows[r].Split(',');
                for (int c = 0; c < size; c++)
                    board._cells[r, c] = cols[c];
            }
            return board;
        }

        public void Render()
        {
            // Columns
            Console.Write("   ");
            for (int c = 0; c < Size; c++) Console.Write($"{c,3}");
            Console.WriteLine();
            for (int r = 0; r < Size; r++)
            {
                Console.Write($"{r,2} ");
                for (int c = 0; c < Size; c++)
                    Console.Write($"{_cells[r, c],3}");
                Console.WriteLine();
            }
        }

        // Check for n-in-a-row 
        public bool HasNInARow(string piece, int n)
        {
            int[,] dirs = { {1,0},{0,1},{1,1},{1,-1} };
            for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
            {
                if (_cells[r, c] != piece) continue;
                for (int d = 0; d < 4; d++)
                {
                    int dr = dirs[d, 0], dc = dirs[d, 1], count = 1;
                    for (int k = 1; k < n; k++)
                    {
                        int nr = r + dr*k, nc = c + dc*k;
                        if (nr < 0 || nr >= Size || nc < 0 || nc >= Size) break;
                        if (_cells[nr, nc] == piece) count++; else break;
                    }
                    if (count >= n) return true;
                }
            }
            return false;
        }

        public bool IsFull()
        {
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_cells[r, c] == ".") return false;
            return true;
        }

        public List<(int x, int y)> EmptyCells()
        {
            var list = new List<(int,int)>();
            for (int r = 0; r < Size; r++)
                for (int c = 0; c < Size; c++)
                    if (_cells[r, c] == ".") list.Add((c, r));
            return list;
        }
    }

    //  MultiBoard (Notakto: 3 × 3×3) 
    public class MultiBoard : IBoard
    {
        public GridBoard[] Boards { get; }
        public int ActiveBoards  => Boards.Count(b => !b.Dead);
        private int _routeIndex; // which sub-board is targeted by a move

        public MultiBoard()
        {
            Boards = new[] { new GridBoard(3), new GridBoard(3), new GridBoard(3) };
        }

        private MultiBoard(MultiBoard src)
        {
            Boards      = src.Boards.Select(b => (GridBoard)b.Clone()).ToArray();
            _routeIndex = src._routeIndex;
        }

        // IBoard routes to sub-board selected by boardIndex on Move
        public void SetRouteIndex(int i) => _routeIndex = i;
        public bool   IsEmpty(int x, int y) => Boards[_routeIndex].IsEmpty(x, y);
        public string GetCell(int x, int y)  => Boards[_routeIndex].GetCell(x, y);
        public void   SetCell(int x, int y, string v) => Boards[_routeIndex].SetCell(x, y, v);
        public IBoard Clone() => new MultiBoard(this);

        public string Serialise() =>
            string.Join("~", Boards.Select(b => b.Serialise()));

        public static MultiBoard Deserialise(string data)
        {
            var mb     = new MultiBoard();
            var parts  = data.Split('~');
            for (int i = 0; i < 3; i++)
                mb.Boards[i] = GridBoard.Deserialise(parts[i]);
            return mb;
        }

        public void Render()
        {
            for (int i = 0; i < 3; i++)
            {
                string status = Boards[i].Dead ? "[DEAD]" : "[LIVE]";
                Console.WriteLine($"  Board {i} {status}");
                Boards[i].Render();
                Console.WriteLine();
            }
        }
    }

    //  NumericBoard 
    // Stores numeric values (1-9) in a 3×3 grid for Numerical TTT
    public class NumericBoard : IBoard
    {
        private readonly string[,] _cells = new string[3, 3];

        public NumericBoard()
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    _cells[r, c] = ".";
        }

        private NumericBoard(NumericBoard src) { _cells = (string[,])src._cells.Clone(); }

        public bool   IsEmpty(int x, int y) => _cells[y, x] == ".";
        public string GetCell(int x, int y)  => _cells[y, x];
        public void   SetCell(int x, int y, string v) => _cells[y, x] = v;
        public IBoard Clone() => new NumericBoard(this);

        public HashSet<int> UsedNumbers()
        {
            var used = new HashSet<int>();
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (_cells[r, c] != "." && int.TryParse(_cells[r, c], out int v))
                        used.Add(v);
            return used;
        }

        public bool IsFull()
        {
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (_cells[r, c] == ".") return false;
            return true;
        }

        // Check if any row/col/diag sums to 15
        public bool AnyLineSums15()
        {
            int V(int r, int c) => _cells[r, c] == "." ? 0 : int.Parse(_cells[r, c]);
            int Has(int r, int c) => _cells[r, c] == "." ? 0 : 1;
            // rows
            for (int r = 0; r < 3; r++)
                if (Has(r,0)+Has(r,1)+Has(r,2)==3 && V(r,0)+V(r,1)+V(r,2)==15) return true;
            // cols
            for (int c = 0; c < 3; c++)
                if (Has(0,c)+Has(1,c)+Has(2,c)==3 && V(0,c)+V(1,c)+V(2,c)==15) return true;
            // diags
            if (Has(0,0)+Has(1,1)+Has(2,2)==3 && V(0,0)+V(1,1)+V(2,2)==15) return true;
            if (Has(0,2)+Has(1,1)+Has(2,0)==3 && V(0,2)+V(1,1)+V(2,0)==15) return true;
            return false;
        }

        public string Serialise()
        {
            var rows = new List<string>();
            for (int r = 0; r < 3; r++)
            {
                var cols = new List<string>();
                for (int c = 0; c < 3; c++) cols.Add(_cells[r, c]);
                rows.Add(string.Join(",", cols));
            }
            return string.Join(";", rows);
        }

        public static NumericBoard Deserialise(string data)
        {
            var board = new NumericBoard();
            var rows  = data.Split(';');
            for (int r = 0; r < 3; r++)
            {
                var cols = rows[r].Split(',');
                for (int c = 0; c < 3; c++) board._cells[r, c] = cols[c];
            }
            return board;
        }

        public void Render()
        {
            Console.WriteLine("   0  1  2");
            for (int r = 0; r < 3; r++)
            {
                Console.Write($"{r}  ");
                for (int c = 0; c < 3; c++)
                    Console.Write($"{_cells[r, c],3}");
                Console.WriteLine();
            }
        }
    }
}
