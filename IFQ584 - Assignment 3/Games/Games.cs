using BoardGames.Interfaces;
using BoardGames.Core;
using BoardGames.Boards;
using BoardGames.Rules;
using BoardGames.Players;

namespace BoardGames.Games
{
    // GameMode 
    public enum GameMode { HumanVsHuman, HumanVsComputer }

    // ═══════════════════════════════════════════════════════════════════════════
    // GomokuGame
    // ═══════════════════════════════════════════════════════════════════════════
    public class GomokuGame : Game
    {
        public override string GameTypeId => "Gomoku";
        private GridBoard GBoard => (GridBoard)Board;
        private GomokuRules GRules => (GomokuRules)Rules;

        public GomokuGame(GameMode mode, int boardSize = 15)
        {
            Mode  = mode.ToString();
            Board = new GridBoard(boardSize);
            Rules = new GomokuRules(5);
            Players = mode == GameMode.HumanVsHuman
                ? new IPlayer[] { new HumanPlayer(1, "p1"), new HumanPlayer(2, "p2") }
                : new IPlayer[] { new HumanPlayer(1, "p1"), new ComputerPlayer(2) };
        }

        public override List<Move> GetLegalMoves() =>
            Rules.LegalMoves(Board, CurrentPlayer.Id);

        public override bool ApplyMove(Move move)
        {
            if (!Rules.IsValid(move, Board)) return false;
            Board.SetCell(move.X, move.Y, move.ValueOrPiece);
            return true;
        }

        public override bool UndoMove(Move move)
        {
            Board.SetCell(move.X, move.Y, ".");
            return true;
        }

        public override GameResult CheckResult() => Rules.Evaluate(Board);
        public override void RenderBoard() => Board.Render();

        public override GameState Serialise() => new()
        {
            GameTypeId    = GameTypeId,
            Mode          = Mode,
            BoardData     = GBoard.Serialise(),
            CurrentPlayer = CurrentPlayerIndex
        };

        public override void RestoreFrom(GameState gs)
        {
            Board               = GridBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex  = gs.CurrentPlayer;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NumericalTTTGame
    // ═══════════════════════════════════════════════════════════════════════════
    public class NumericalTTTGame : Game
    {
        public override string GameTypeId => "NumericalTTT";
        private NumericBoard NBoard => (NumericBoard)Board;

        public NumericalTTTGame(GameMode mode)
        {
            Mode  = mode.ToString();
            Board = new NumericBoard();
            Rules = new NumericalTTTRules();
            Players = mode == GameMode.HumanVsHuman
                ? new IPlayer[] { new HumanPlayer(1, "p1"), new HumanPlayer(2, "p2") }
                : new IPlayer[] { new HumanPlayer(1, "p1"), new ComputerPlayer(2) };
        }

        public override List<Move> GetLegalMoves() =>
            Rules.LegalMoves(Board, CurrentPlayer.Id);

        public override bool ApplyMove(Move move)
        {
            if (!Rules.IsValid(move, Board)) return false;
            Board.SetCell(move.X, move.Y, move.ValueOrPiece);
            return true;
        }

        public override bool UndoMove(Move move)
        {
            Board.SetCell(move.X, move.Y, ".");
            return true;
        }

        public override GameResult CheckResult() => Rules.Evaluate(Board);
        public override void RenderBoard() => Board.Render();

        public override GameState Serialise() => new()
        {
            GameTypeId    = GameTypeId,
            Mode          = Mode,
            BoardData     = NBoard.Serialise(),
            CurrentPlayer = CurrentPlayerIndex
        };

        public override void RestoreFrom(GameState gs)
        {
            Board              = NumericBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex = gs.CurrentPlayer;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // NotaktoGame
    // ═══════════════════════════════════════════════════════════════════════════
    public class NotaktoGame : Game
    {
        public override string GameTypeId => "Notakto";
        private MultiBoard MBoard => (MultiBoard)Board;
        private NotaktoRules NRules => (NotaktoRules)Rules;

        public NotaktoGame(GameMode mode)
        {
            Mode  = mode.ToString();
            Board = new MultiBoard();
            Rules = new NotaktoRules();
            Players = mode == GameMode.HumanVsHuman
                ? new IPlayer[] { new HumanPlayer(1, "p1"), new HumanPlayer(2, "p2") }
                : new IPlayer[] { new HumanPlayer(1, "p1"), new ComputerPlayer(2) };
        }

        public override List<Move> GetLegalMoves() =>
            Rules.LegalMoves(Board, CurrentPlayer.Id);

        public override bool ApplyMove(Move move)
        {
            if (!Rules.IsValid(move, Board)) return false;
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
            foreach (var gb in MBoard.Boards)
                if (!gb.Dead && gb.HasNInARow("X", 3)) gb.Dead = true;
            return true;
        }

        public override GameResult CheckResult() => Rules.Evaluate(Board);
        public override void RenderBoard() => Board.Render();

        public override GameState Serialise() => new()
        {
            GameTypeId    = GameTypeId,
            Mode          = Mode,
            BoardData     = MBoard.Serialise(),
            CurrentPlayer = CurrentPlayerIndex
        };

        public override void RestoreFrom(GameState gs)
        {
            Board              = MultiBoard.Deserialise(gs.BoardData);
            CurrentPlayerIndex = gs.CurrentPlayer;
        }
    }

    // ═══════════════════════════════════════════════════════════════════════════
    // GameFactory
    // ═══════════════════════════════════════════════════════════════════════════
    public static class GameFactory
    {
        public static Game Create(string typeId, GameMode mode) => typeId switch
        {
            "Gomoku"       => new GomokuGame(mode),
            "NumericalTTT" => new NumericalTTTGame(mode),
            "Notakto"      => new NotaktoGame(mode),
            _              => throw new ArgumentException($"Unknown game type: {typeId}")
        };
    }
}
