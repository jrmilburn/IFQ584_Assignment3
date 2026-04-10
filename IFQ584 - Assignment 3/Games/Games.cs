namespace TicTacToe_Framework
{
    public abstract class Game
    {
        protected IBoard Board { get; set; } = null!;  // ---------------------------------------------------
        protected IRules Rules { get; set; } = null!;    // core dependencies for games, protected so they cannot be modified outside of the class.
        protected IHelp Help { get; set; } = null!;
        protected Player[] Players { get; set; } = null!;  // -----------------------------------------------
        public int CurrentPlayerIndex { get; set; }
        public Player CurrentPlayer => Players[CurrentPlayerIndex];
        public string Mode { get; protected set; } = "";  // Stores whether this is HumanVsHuman or HumanVsComputer
        public abstract string GameTypeId { get; }  // Unique identifier for the game type, used by GameFactory and SaveLoadService
        public Move[] GetLegalMoves() => Rules.GetAvailableMoves(Board, CurrentPlayer.ID);
        public abstract Move? ParseMove(string[] args, int playerId);
        public virtual bool ApplyMove(Move move)   // ----------------------------------------------
        {
            if (!Rules.IsValid(move, Board, CurrentPlayer.ID)) return false;  // delegate validation to the Rules, Game never check the rules directly
            Board.SetCell(move.X, move.Y, move.ValueOrPiece);
            return true;
        }                                          // ----------------------------------------------
        public virtual bool UndoMove(Move move)   // reverses move by clearing cell, virtual so can be overriden by games like Notakto which also need to revive boards
        {
            Board.SetCell(move.X, move.Y, ".");
            return true;
        }
        public GameResult CheckResult() => Rules.Evaluate(Board);  // delegate result evaluation to the Rules
        public void RenderBoard() => Board.Render(); // delegate rendering to the Board
        public GameState Serialise() => new()  // ---------------------------------------------
        {
            GameTypeId = GameTypeId,
            Mode = Mode,                        // captures current game state as a GameState object, which can be used for saving/loading and undo/redo functionality. 
            BoardData = Board.Serialise(),
            CurrentPlayer = CurrentPlayerIndex
        };                                     // ---------------------------------------------
        public virtual void RestoreFrom(GameState gs)  // Restores board and player state from a saved GameState. Virtual so NotaktoGame can override to restore its MultiBoard instead of GridBoard
        {
            Board = GridBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex = gs.CurrentPlayer;
        }
        public void NextPlayer() =>
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Length;
        public void ShowHelp()    // Calls the game-specific help class to display instructions
        {
            Console.WriteLine(Help.ShowHelp());
        }
    }
    // GameMode 
    public enum GameMode { HumanVsHuman, HumanVsComputer }

    // ═══════════════════════════════════════════════════════════════════════════
    // GomokuGame, Concrete implementation of Game for Gomoku, Inherits ApplyMove, UndoMove, Serialise and RestoreFrom from Game as the default GridBoard behaviour is sufficient.
    // ═══════════════════════════════════════════════════════════════════════════
    public class GomokuGame : Game
    {
        public override string GameTypeId => "Gomoku";  // Initialises a Gomoku game with a 15x15 GridBoard and GomokuRules.
        public GomokuGame(GameMode mode, int boardSize = 15)  // boardSize defaults to 15 but can be overridden if needed.
        {
            Mode = mode.ToString();
            Board = new GridBoard(boardSize);
            Help = new GomokuHelp();
            Rules = new GomokuRules();
            Players = mode == GameMode.HumanVsHuman
                ? new Player[] { new HumanPlayer(1, "Player 1"), new HumanPlayer(2, "Player 2") }
                : new Player[] { new HumanPlayer(1, "Player 1"), new ComputerPlayer(2) };
        }

        public override Move? ParseMove(string[] args, int playerId)  // Parses a Gomoku move from args: expects col and row only.
        {
            if (args.Length < 2) return null;  // Returns null if args are missing or not valid integers.
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y)) return null;
            string piece = playerId == 1 ? "X" : "O";  // Player 1 uses "X", Player 2 uses "O".
            return new Move(playerId, x, y, piece);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NumericalTTTGame, Concrete implementation of Game for Numerical Tic-Tac-Toe. Board size is configurable — rules scale automatically to match.
    // ═══════════════════════════════════════════════════════════════════════════
    public class NumericalTTTGame : Game
    {
        public override string GameTypeId => "NumericalTTT";
        public NumericalTTTGame(GameMode mode, int boardSize = 3) // Initialises with a user-defined board size (default 3x3).
        {
            Mode = mode.ToString();
            Board = new GridBoard(boardSize);   // Both GridBoard and NumericalTTTRules receive the same boardSize so the board dimensions and winning score stay in sync.
            Rules = new NumericalTTTRules(boardSize);
            Help = new NumTTTHelp();
            Players = mode == GameMode.HumanVsHuman
                ? new Player[] { new HumanPlayer(1, "Player 1"), new HumanPlayer(2, "Player 2") }
                : new Player[] { new HumanPlayer(1, "Player 1"), new ComputerPlayer(2) };
        }

        public override Move? ParseMove(string[] args, int playerId) // Parses a NumericalTTT move from args: expects col, row and number. The number is passed as a string and validated by NumericalTTTRules.IsValid.
        {
            if (args.Length < 3) return null;     // Returns null if args are missing or coordinates are not valid integers.
            if (!int.TryParse(args[0], out int x) || !int.TryParse(args[1], out int y)) return null;
            return new Move(playerId, x, y, args[2]);
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NotaktoGame, Concrete implementation of Game for Notakto. Overrides ApplyMove, UndoMove and RestoreFrom because Notakto uses a MultiBoard (three GridBoards) instead of a single GridBoard.
    // ═══════════════════════════════════════════════════════════════════════════
    public class NotaktoGame : Game
    {
        public override string GameTypeId => "Notakto";
        private MultiBoard MBoard => (MultiBoard)Board;  // Convenience property to access Board as a MultiBoard without casting every time
        public NotaktoGame(GameMode mode)
        {
            Mode = mode.ToString();
            Board = new MultiBoard();
            Rules = new NotaktoRules();
            Help = new NotaktoHelp();
            Players = mode == GameMode.HumanVsHuman
                ? new Player[] { new HumanPlayer(1, "Player 1"), new HumanPlayer(2, "Player 2") }
                : new Player[] { new HumanPlayer(1, "Player 1"), new ComputerPlayer(2) };
        }

        public override Move? ParseMove(string[] args, int playerId) // Parses a Notakto move from args: expects col, row and boardIndex.
        {
            if (args.Length < 3) return null;      // Returns null if args are missing or any value is not a valid integer.
            if (!int.TryParse(args[0], out int x)) return null;
            if (!int.TryParse(args[1], out int y)) return null;
            if (!int.TryParse(args[^1], out int boardIndex)) return null; // computer and human outputs will be different, this ensures the last value in a move command is read
            return new Move(playerId, x, y, "X", boardIndex);
        }
        public override bool ApplyMove(Move move)  // Overrides ApplyMove to route the move to the correct sub-board and trigger board death evaluation after each placement.
        {
            if (!Rules.IsValid(move, Board, CurrentPlayer.ID)) return false;
            MBoard.SetRouteIndex(move.BoardIndex);  // direct IBoard calls to correct sub-board
            Board.SetCell(move.X, move.Y, move.ValueOrPiece);
            Rules.Evaluate(Board); // updates dead boards
            return true;
        }

        public override bool UndoMove(Move move)  // Overrides UndoMove to reverse a move on the correct sub-board. Revives the board first before re-evaluating so dead status is recalculated correctly from the restored board state.
        {
            MBoard.SetRouteIndex(move.BoardIndex);
            Board.SetCell(move.X, move.Y, ".");
            // Revive board if it was killed by this move
            MBoard.Boards[move.BoardIndex].Dead = false;
            // Re-evaluate all boards for dead status
            Rules.Evaluate(Board);
            return true;
        }

        public override void RestoreFrom(GameState gs)   // Overrides RestoreFrom to deserialise a MultiBoard instead of a GridBoard
        {
            Board = MultiBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex = gs.CurrentPlayer;
        }


    }

    // ═══════════════════════════════════════════════════════════════════════════
    // GameFactory, Factory class responsible for creating the correct Game instance. Used by SaveLoadService to restore saved games and can be used by the main program to create new games based on user input.
    // Each game type is identified by a unique string GameTypeId, which is used to determine which Game subclass to instantiate. 
    // ═══════════════════════════════════════════════════════════════════════════
    public static class GameFactory
    {
        public static Game Create(string typeId, GameMode mode, int boardSize = 3) => typeId switch  // boardSize is only used by NumericalTTT — other games ignore it.
        {
            "Gomoku" => new GomokuGame(mode),
            "NumericalTTT" => new NumericalTTTGame(mode, boardSize),
            "Notakto" => new NotaktoGame(mode),
            _ => throw new ArgumentException($"Unknown game type: {typeId}")
        };
    }
}