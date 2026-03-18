namespace BoardGames.Interfaces
{
    //  IBoard 
    public interface IBoard
    {
        bool IsEmpty(int x, int y);
        string GetCell(int x, int y);
        void SetCell(int x, int y, string value);
        IBoard Clone();
        string Serialise();
        void Render();
    }

    //  IRules 
    public interface IRules
    {
        bool IsValid(Core.Move move, IBoard board);
        List<Core.Move> LegalMoves(IBoard board, int playerId);
        GameResult Evaluate(IBoard board);
    }

    //  IPlayer 
    public interface IPlayer
    {
        int Id { get; }
        Core.Move GetMove(Core.Game game);
    }

    //  GameResult 
    public enum GameResult { NotFinished, Win, Loss, Tie }
}
