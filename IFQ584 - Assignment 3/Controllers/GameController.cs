using static System.Console;
namespace TicTacToe_Framework
{
    public class GameController    // GameController is the central coordinator of the program.
    {                              // It runs the main game loop and routes player commands to the correct handlers. It is fully game-agnostic — it never references any concrete game type, board type or rules class directly.
                                   // All game-specific behaviour is handled through the abstract Game class.
        private Game            _game;  // The current game being played — stored as the abstract Game type so GameController works identically for all three games
        private MoveHistory     _history;  // Tracks all moves made this session to support undo/redo
        private SaveLoadService _saveLoad; // Handles saving and loading game state to/from disk

        public GameController(Game game)  // Constructor receives a fully configured Game from GameFactory.
        {
            _game     = game;
            _history  = new MoveHistory();
            _saveLoad = new SaveLoadService();
        }

        public void Run()   // Main game loop. Runs until the game ends or the player quits. Each iteration: renders the board, checks for a result, gets a command from the current player and handles it.
        {
            Console.WriteLine($"\n  === {_game.GameTypeId} ({_game.Mode}) ===");
            _game.ShowHelp();  // Display game-specific instructions at the start of each session

            while (true)
            {
                _game.RenderBoard();  // Always render the board at the start of each turn so the player can see the current state
                Console.WriteLine($"\n  Turn: {_game.CurrentPlayer.Name}");

                var result = _game.CheckResult(); // Check result before asking for input - if the game just ended, we want to announce it instead of asking for another move
                if (result != GameResult.NotFinished)
                {
                    AnnounceResult(result);  // Announce the result if the game has ended
                    break;
                }
                Command cmd =_game.CurrentPlayer.GetCommand(_game);  // Get command from the current player, this is polymorphic and will call the correct method for human vs AI players
                if (!HandleCommand(cmd)) break; // quit
            }
        }
        private bool HandleCommand(Command cmd)  // Routes a command to the appropriate handler method.
        {
            switch (cmd.Type)
            {
                case CommandType.Quit:
                    Console.WriteLine("  Goodbye!");
                    return false;  // signals Run() to exit the loop

                case CommandType.Help: // Delegates to the game's IHelp implementation - GameController never holds or knows the help content
                    _game.ShowHelp();
                    return true;

                case CommandType.Undo:
                    PerformUndo();
                    return true;

                case CommandType.Redo:
                    PerformRedo();
                    return true;

                case CommandType.Save: 
                    string savePath = cmd.Input.Length > 1 ? cmd.Input[1] : "save.json";  // Uses provided filename or defaults to "save.json"
                    _saveLoad.Save(savePath, _game.Serialise(), _history); // Passes both game state and move history so everything can be fully restored on load
                    return true;

                case CommandType.Load:
                    string loadPath = cmd.Input.Length > 1 ? cmd.Input[1] : "save.json";
                    var loaded = _saveLoad.Load(loadPath);
                    if (loaded.HasValue)  // Replace both the game and history with the restored versions so the session continues exactly where it was saved
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
            // Delegate move parsing to the game — controller stays game-agnostic, each game knows its own format via ParseMove.
            var move = _game.ParseMove(args, _game.CurrentPlayer.ID); // Delegate move parsing to the game
            if (move == null) // ParseMove returns null if args are missing or malformed
            {
                Console.WriteLine("  Invalid input. Type HELP for move format.");
                return;
            }

            if (_game.ApplyMove(move)) // ApplyMove validates and places the piece - returns false if the move breaks the rules
            {
                _history.DoMove(move);   // Only record to history if the move was successfully applied
                Console.WriteLine($"  Move recorded: {move}");

                var result = _game.CheckResult();  // Check if the move ended the game before advancing the turn
                if (result == GameResult.NotFinished)  // If the game is finished, Run() will catch it on the next iteration
                    _game.NextPlayer();
            }
            else
            {
                Console.WriteLine("  Invalid move. Try again.");
            }
        }

        private void PerformUndo() // Undoes the most recent move by popping from the done stack and reversing it on the board.
        {
            var move = _history.Undo();
            if (move == null) { Console.WriteLine("  Nothing to undo."); return; } // MoveHistory returns null if there is nothing to undo
            _game.UndoMove(move);
            // Step back one player
            int prev = (_game.CurrentPlayerIndex + _game.GetLegalMoves().Length > 0 ? 1 : 0);
            // Simple: toggle between 0 and 1
            _game.CurrentPlayerIndex = 1 - _game.CurrentPlayerIndex;
            Console.WriteLine($"  Undo: {move}");
        }

        private void PerformRedo() // Redoes the most recently undone move by popping from the undone stack and reapplying it on the board.
        {
            var move = _history.Redo();
            if (move == null) { Console.WriteLine("  Nothing to redo."); return; }
            _game.ApplyMove(move);
            _game.NextPlayer();
            Console.WriteLine($"  Redo: {move}");
        }

        private void AnnounceResult(GameResult result)  // Displays the final board state and announces the game result. Called once when CheckResult returns anything other than NotFinished.
        {
            _game.RenderBoard(); // Show the final board before announcing the result
            Console.WriteLine();
            switch (result)
            {
                case GameResult.Win:
                    Console.WriteLine($"  🎉 Player {_game.CurrentPlayer.ID} wins!");
                    break;
                case GameResult.Loss:  // Loss is used by Notakto's misère rule — the last player to complete a line loses
                    Console.WriteLine($"  Player {_game.CurrentPlayer.ID} loses (misère).");
                    break;
                case GameResult.Tie:
                    Console.WriteLine("  It's a tie!");
                    break;
            }
        }
    }
}
