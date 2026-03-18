using BoardGames.Core;
using BoardGames.Games;
using BoardGames.Interfaces;
using BoardGames.Players;
using BoardGames.Services;

namespace BoardGames.Controllers
{
    public class GameController
    {
        private Game            _game;
        private MoveHistory     _history;
        private CommandParser   _parser;
        private SaveLoadService _saveLoad;

        public GameController(Game game)
        {
            _game     = game;
            _history  = new MoveHistory();
            _parser   = new CommandParser();
            _saveLoad = new SaveLoadService();
        }

        public void Run()
        {
            Console.WriteLine($"\n  === {_game.GameTypeId} ({_game.Mode}) ===");
            _parser.ShowHelp();

            while (true)
            {
                _game.RenderBoard();
                Console.WriteLine($"\n  Turn: Player {_game.CurrentPlayer.Id}");

                var result = _game.CheckResult();
                if (result != GameResult.NotFinished)
                {
                    AnnounceResult(result);
                    break;
                }

                //  Computer turn 
                if (_game.CurrentPlayer is ComputerPlayer cp)
                {
                    var cpMove = cp.GetMove(_game);
                    if (_game.ApplyMove(cpMove))
                    {
                        _history.DoMove(cpMove);
                        _game.NextPlayer();
                    }
                    continue;
                }

                //  Human turn 
                var humanMove = _game.CurrentPlayer.GetMove(_game);
                // humanMove.ValueOrPiece contains raw input
                var cmd = _parser.Parse(humanMove.ValueOrPiece);

                if (!HandleCommand(cmd)) break; // quit
            }
        }

        private bool HandleCommand(Command cmd)
        {
            switch (cmd.Type)
            {
                case CommandType.Quit:
                    Console.WriteLine("  Goodbye!");
                    return false;

                case CommandType.Help:
                    _parser.ShowHelp();
                    return true;

                case CommandType.Undo:
                    PerformUndo();
                    return true;

                case CommandType.Redo:
                    PerformRedo();
                    return true;

                case CommandType.Save:
                    string savePath = cmd.Args.Length > 1 ? cmd.Args[1] : "save.json";
                    _saveLoad.Save(savePath, _game.Serialise(), _history);
                    return true;

                case CommandType.Load:
                    string loadPath = cmd.Args.Length > 1 ? cmd.Args[1] : "save.json";
                    var loaded = _saveLoad.Load(loadPath);
                    if (loaded.HasValue)
                    {
                        _game    = loaded.Value.game;
                        _history = loaded.Value.history;
                    }
                    return true;

                case CommandType.Move:
                    PerformMove(cmd.Args);
                    return true;

                default:
                    Console.WriteLine("  Invalid command. Type HELP for options.");
                    return true;
            }
        }

        private void PerformMove(string[] args)
        {
            // Parse: MOVE <col> <row> [value/boardIndex]
            // For Notakto: MOVE <col> <row> <boardIndex>
            // For NumericalTTT: MOVE <col> <row> <number>
            // For Gomoku: MOVE <col> <row>
            if (args.Length < 3 || !int.TryParse(args[1], out int col) || !int.TryParse(args[2], out int row))
            {
                Console.WriteLine("  Usage: MOVE <col> <row> [number/boardIndex]");
                return;
            }

            string valueOrPiece = _game.CurrentPlayer.Id == 1 ? "X" : "O";
            int boardIndex = 0;

            if (args.Length >= 4)
            {
                // NumericalTTT expects a number; Notakto expects a board index
                if (_game.GameTypeId == "NumericalTTT")
                    valueOrPiece = args[3];
                else if (_game.GameTypeId == "Notakto" && int.TryParse(args[3], out int bi))
                    boardIndex = bi;
            }

            var move = new Move(_game.CurrentPlayer.Id, col, row, valueOrPiece, boardIndex);

            if (_game.ApplyMove(move))
            {
                _history.DoMove(move);
                Console.WriteLine($"  Move recorded: {move}");

                var result = _game.CheckResult();
                if (result == GameResult.NotFinished)
                    _game.NextPlayer();
            }
            else
            {
                Console.WriteLine("  Invalid move. Try again.");
            }
        }

        private void PerformUndo()
        {
            var move = _history.Undo();
            if (move == null) { Console.WriteLine("  Nothing to undo."); return; }
            _game.UndoMove(move);
            // Step back one player
            int prev = (_game.CurrentPlayerIndex + _game.GetLegalMoves().Count > 0 ? 1 : 0);
            // Simple: toggle between 0 and 1
            _game.CurrentPlayerIndex = 1 - _game.CurrentPlayerIndex;
            Console.WriteLine($"  Undo: {move}");
        }

        private void PerformRedo()
        {
            var move = _history.Redo();
            if (move == null) { Console.WriteLine("  Nothing to redo."); return; }
            _game.ApplyMove(move);
            _game.NextPlayer();
            Console.WriteLine($"  Redo: {move}");
        }

        private void AnnounceResult(GameResult result)
        {
            _game.RenderBoard();
            Console.WriteLine();
            switch (result)
            {
                case GameResult.Win:
                    Console.WriteLine($"  🎉 Player {_game.CurrentPlayer.Id} wins!");
                    break;
                case GameResult.Loss:
                    Console.WriteLine($"  Player {_game.CurrentPlayer.Id} loses (misère).");
                    break;
                case GameResult.Tie:
                    Console.WriteLine("  It's a tie!");
                    break;
            }
        }
    }
}
