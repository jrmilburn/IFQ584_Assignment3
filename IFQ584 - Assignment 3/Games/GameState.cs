namespace BoardGames
{
	public class GameState
	{
		public string GameTypeId { get; set; } = "";
		public string Mode { get; set; } = "";
		public string BoardData { get; set; } = "";
		public int CurrentPlayer { get; set; }
		public List<string> UndoMoves { get; set; } = new();
		public List<string> RedoMoves { get; set; } = new();
	}
}
