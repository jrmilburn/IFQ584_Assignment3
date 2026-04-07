namespace BoardGames
{
    public abstract class Game
    {
        protected IBoard Board { get; set; } = null!;
        protected IRules Rules { get; set; } = null!;
        protected IHelp Help { get; set; } = null!;
        protected Player[] Players { get; set; } = null!;
        public int CurrentPlayerIndex { get; set; }
        public Player CurrentPlayer => Players[CurrentPlayerIndex];
        public string Mode { get; protected set; } = "";
        public abstract string GameTypeId { get; }
        public Move[] GetLegalMoves() => Rules.GetAvailableMoves(Board, CurrentPlayer.ID);
        public abstract Move? ParseMove(string[] args, int playerId);
        public virtual bool ApplyMove(Move move)
        {
            if (!Rules.IsValid(move, Board, CurrentPlayer.ID)) return false;
            Board.SetCell(move.X, move.Y, move.ValueOrPiece);
            return true;
        }
        public virtual bool UndoMove(Move move)
        {
            Board.SetCell(move.X, move.Y, ".");
            return true;
        }
        public GameResult CheckResult() => Rules.Evaluate(Board);
        public void RenderBoard() => Board.Render();
        public GameState Serialise() => new()
        {
            GameTypeId = GameTypeId,
            Mode = Mode,
            BoardData = Board.Serialise(),
            CurrentPlayer = CurrentPlayerIndex
        };
        public virtual void RestoreFrom(GameState gs)
        {
            Board = GridBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex = gs.CurrentPlayer;
        }
        public void NextPlayer() =>
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Length;
        public void ShowHelp()
        {
            Console.WriteLine(Help.ShowHelp());
        }
    }
    // GameMode 
    public enum GameMode { HumanVsHuman, HumanVsComputer }

    // ═══════════════════════════════════════════════════════════════════════════
    // GomokuGame
    // ═══════════════════════════════════════════════════════════════════════════
    public class GomokuGame : Game
    {
        public override string GameTypeId => "Gomoku";
        public GomokuGame(GameMode mode, int boardSize = 15)
        {
            Mode = mode.ToString();
            Board = new GridBoard(boardSize);
            Help = new GomokuHelp();
            Rules = new GomokuRules();
            Players = mode == GameMode.HumanVsHuman
                ? new Player[] { new HumanPlayer(1, "p1"), new HumanPlayer(2, "p2") }
                : new Player[] { new HumanPlayer(1, "p1"), new ComputerPlayer(2) };
        }

        public override Move? ParseMove(string[] args, int playerId)
        {
            if (args.Length < 2) return null;
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y)) return null;
            string piece = playerId == 1 ? "X" : "O";
            return new Move(playerId, x, y, piece);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NumericalTTTGame
    // ═══════════════════════════════════════════════════════════════════════════
    public class NumericalTTTGame : Game
    {
        public override string GameTypeId => "NumericalTTT";
        public NumericalTTTGame(GameMode mode, int boardSize = 3)
        {
            Mode = mode.ToString();
            Board = new GridBoard(boardSize);
            Rules = new NumericalTTTRules(boardSize);
            Help = new NumTTTHelp();
            Players = mode == GameMode.HumanVsHuman
                ? new Player[] { new HumanPlayer(1, "p1"), new HumanPlayer(2, "p2") }
                : new Player[] { new HumanPlayer(1, "p1"), new ComputerPlayer(2) };
        }

        public override Move? ParseMove(string[] args, int playerId)
        {
            if (args.Length < 3) return null;
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y)) return null;
            return new Move(playerId, x, y, args[2]);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NotaktoGame
    // ═══════════════════════════════════════════════════════════════════════════
    public class NotaktoGame : Game
    {
        public override string GameTypeId => "Notakto";
        private MultiBoard MBoard => (MultiBoard)Board;
        public NotaktoGame(GameMode mode)
        {
            Mode = mode.ToString();
            Board = new MultiBoard();
            Rules = new NotaktoRules();
            Help = new NotaktoHelp();
            Players = mode == GameMode.HumanVsHuman
                ? new Player[] { new HumanPlayer(1, "p1"), new HumanPlayer(2, "p2") }
                : new Player[] { new HumanPlayer(1, "p1"), new ComputerPlayer(2) };
        }

        public override Move? ParseMove(string[] args, int playerId)
        {
            if (args.Length < 3) return null;
            if (!int.TryParse(args[0], out int x)) return null;
            if (!int.TryParse(args[1], out int y)) return null;
            if (!int.TryParse(args[^1], out int boardIndex)) return null; // computer and human outputs will be different, this ensures the last value in a move command is read
            return new Move(playerId, x, y, "X", boardIndex);
        }
        public override bool ApplyMove(Move move)
        {
            if (!Rules.IsValid(move, Board, CurrentPlayer.ID)) return false;
            MBoard.SetRouteIndex(move.BoardIndex);
            Board.SetCell(move.X, move.Y, move.ValueOrPiece);
            Rules.Evaluate(Board); // updates dead boards
            return true;
        }

        public override bool UndoMove(Move move)
        {
            MBoard.SetRouteIndex(move.BoardIndex);
            Board.SetCell(move.X, move.Y, ".");
            // Revive board if it was killed by this move
            MBoard.Boards[move.BoardIndex].Dead = false;
            // Re-evaluate all boards for dead status
            Rules.Evaluate(Board);
            return true;
        }

        public override void RestoreFrom(GameState gs)
        {
            Board = MultiBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex = gs.CurrentPlayer;
        }


    }

    // ═══════════════════════════════════════════════════════════════════════════
    // GameFactory
    // ═══════════════════════════════════════════════════════════════════════════
    public static class GameFactory
    {
        public static Game Create(string typeId, GameMode mode, int boardSize = 3) => typeId switch
        {
            "Gomoku" => new GomokuGame(mode),
            "NumericalTTT" => new NumericalTTTGame(mode, boardSize),
            "Notakto" => new NotaktoGame(mode),
            _ => throw new ArgumentException($"Unknown game type: {typeId}")
        };
    }
}