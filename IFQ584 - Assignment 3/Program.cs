namespace BoardGames
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

                var game = GameFactory.Create(gameType, mode);
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