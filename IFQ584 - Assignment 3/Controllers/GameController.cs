using static System.Console;
namespace TicTacToe_Framewrork
{
    public class GameController
    {
        private Game            _game;
        private MoveHistory     _history;
        private SaveLoadService _saveLoad;

        public GameController(Game game)
        {
            _game     = game;
            _history  = new MoveHistory();
            _saveLoad = new SaveLoadService();
        }

        public void Run()
        {
            Console.WriteLine($"\n  === {_game.GameTypeId} ({_game.Mode}) ===");
            _game.ShowHelp();

            while (true)
            {
                _game.RenderBoard();
                Console.WriteLine($"\n  Turn: Player {_game.CurrentPlayer.ID}");

                var result = _game.CheckResult();
                if (result != GameResult.NotFinished)
                {
                    AnnounceResult(result);
                    break;
                }
                Command cmd =_game.CurrentPlayer.GetCommand(_game);
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
                    _game.ShowHelp();
                    return true;

                case CommandType.Undo:
                    PerformUndo();
                    return true;

                case CommandType.Redo:
                    PerformRedo();
                    return true;

                case CommandType.Save:
                    string savePath = cmd.Input.Length > 1 ? cmd.Input[1] : "save.json";
                    _saveLoad.Save(savePath, _game.Serialise(), _history);
                    return true;

                case CommandType.Load:
                    string loadPath = cmd.Input.Length > 1 ? cmd.Input[1] : "save.json";
                    var loaded = _saveLoad.Load(loadPath);
                    if (loaded.HasValue)
                    {
                        _game    = loaded.Value.game;
                        _history = loaded.Value.history;
                    }
                    return true;

                case CommandType.Move:
					PerformMove(cmd.Input);
                    return true;

                default:
                    Console.WriteLine("  Invalid command. Type HELP for options.");
                    return true;
            }
        }

        private void PerformMove(string[] args)
        {
			// Delegate move parsing to the game — controller stays game-agnostic
			var move = _game.ParseMove(args, _game.CurrentPlayer.ID);
            if (move == null)
            {
                Console.WriteLine("  Invalid input. Type HELP for move format.");
                return;
            }

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
            int prev = (_game.CurrentPlayerIndex + _game.GetLegalMoves().Length > 0 ? 1 : 0);
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
                    Console.WriteLine($"  🎉 Player {_game.CurrentPlayer.ID} wins!");
                    break;
                case GameResult.Loss:
                    Console.WriteLine($"  Player {_game.CurrentPlayer.ID} loses (misère).");
                    break;
                case GameResult.Tie:
                    Console.WriteLine("  It's a tie!");
                    break;
            }
        }
    }
}
