using BoardGames.Interfaces;
using BoardGames.Core;

namespace BoardGames.Players
{
    // HumanPlayer 
    public class HumanPlayer : IPlayer
    {
        public int    Id   { get; }
        public string Name { get; }

        public HumanPlayer(int id, string name) { Id = id; Name = name; }

        public Move GetMove(Game game)
        {
            // collect a raw line
            //  delegate validation back through game.GetLegalMoves / IRules.
            while (true)
            {
                Console.Write($"  {Name} > ");
                var input = Console.ReadLine()?.Trim() ?? "";

               
                // embed the raw input in a special "human input" move using
                // x=y=-1 as a marker; GameController handles it.
                return new Move(Id, -1, -1, input);
            }
        }
    }

    //  ComputerPlayer 
    public class ComputerPlayer : IPlayer
    {
        public int Id  { get; }
        private readonly Random _rng;

        public ComputerPlayer(int id, Random? rng = null)
        {
            Id   = id;
            _rng = rng ?? new Random();
        }

        public Move GetMove(Game game)
        {
            var legal = game.GetLegalMoves();
            if (legal.Count == 0) throw new InvalidOperationException("No legal moves.");

            // Try immediate win
            var winning = FindImmediateWin(game, legal);
            if (winning != null)
            {
                Console.WriteLine("  [CPU] Found winning move!");
                return winning;
            }

            // Random fallback
            var chosen = legal[_rng.Next(legal.Count)];
            Console.WriteLine($"  [CPU] Plays ({chosen.X},{chosen.Y})");
            return chosen;
        }

        private Move? FindImmediateWin(Game game, List<Move> candidates)
        {
            foreach (var m in candidates)
            {
                // Use game.ApplyMove / CheckResult / UndoMove to test
                game.ApplyMove(m);
                var result = game.CheckResult();
                game.UndoMove(m);
                if (result == GameResult.Win || result == GameResult.Loss)
                    return m;
            }
            return null;
        }
    }
}
