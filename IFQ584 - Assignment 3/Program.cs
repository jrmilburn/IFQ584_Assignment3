namespace TicTacToe_Framewrork
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("╔══════════════════════════════════════════╗");
            Console.WriteLine("║        IFQ584  -  Assignment 3           ║");
            Console.WriteLine("╚══════════════════════════════════════════╝");

            while (true)
            {
                var gameType = SelectGame();
                if (gameType == null) break;

                var mode = SelectMode();

                int boardSize = gameType == "NumericalTTT" ? SelectBoardSize() : 3;
                var game = GameFactory.Create(gameType, mode, boardSize);
                var controller = new GameController(game);
                controller.Run();

                Console.Write("\n  Play again? (y/n): ");
                if (Console.ReadLine()?.Trim().ToLower() != "y") break;
            }

            Console.WriteLine("\n  Thanks for playing!");
        }

        static string? SelectGame()
        {
            Console.WriteLine("\n  Select a game:");
            Console.WriteLine("    1. Gomoku");
            Console.WriteLine("    2. Numerical Tic-Tac-Toe");
            Console.WriteLine("    3. Notakto");
            Console.WriteLine("    4. Quit");
            Console.Write("  > ");

            return Console.ReadLine()?.Trim() switch
            {
                "1" => "Gomoku",
                "2" => "NumericalTTT",
                "3" => "Notakto",
                _ => null
            };
        }
        static int SelectBoardSize()
        {
            Console.WriteLine("\n  Enter board size (e.g. 3 for 3x3, 4 for 4x4):");
            Console.Write("  > ");
            if (int.TryParse(Console.ReadLine()?.Trim(), out int size) && size >= 3)
                return size;
            Console.WriteLine("  Invalid input, defaulting to 3.");
            return 3;
        }

        static GameMode SelectMode()
        {
            Console.WriteLine("\n  Select mode:");
            Console.WriteLine("    1. Human vs Human");
            Console.WriteLine("    2. Human vs Computer");
            Console.Write("  > ");

            return Console.ReadLine()?.Trim() switch
            {
                "2" => GameMode.HumanVsComputer,
                _ => GameMode.HumanVsHuman
            };
        }
    }
}