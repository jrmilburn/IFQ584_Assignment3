namespace TicTacToe_Framework
// GameState is a data container that captures a complete snapshot
// of a game session at a point in time. It is the Memento in the
// Memento Pattern — it stores everything SaveLoadService needs to
// fully reconstruct a game without exposing any internal game logic.
// Game, MoveHistory and the board classes never interact with
// GameState directly — only Game.Serialise() writes to it and
// SaveLoadService reads from it.
{
    public class GameState
	{
        // Identifies which game type to reconstruct on load
        // (e.g. "Gomoku", "NumericalTTT", "Notakto").
        // Used by GameFactory.Create() to instantiate the correct game class.
        public string GameTypeId { get; set; } = "";
        // Stores whether the saved session was HumanVsHuman or HumanVsComputer.
        // Parsed back into a GameMode enum on load so the correct
        // player types are reconstructed.
        public string Mode { get; set; } = "";
        // The serialised board state — a string representation of all
        // cell values at the time of saving. Each board type (GridBoard,
        // MultiBoard) has its own Serialise/Deserialise format.
        // GameState stores it as a plain string so it remains
        // board-type agnostic.
        public string BoardData { get; set; } = "";
        // The index of the player whose turn it was when the game was saved.
        // Restored directly into Game.CurrentPlayerIndex on load so the
        // correct player takes the next turn.
        public int CurrentPlayer { get; set; }
        // The undo stack serialised as a list of comma-separated move strings.
        // Format: "playerId,x,y,valueOrPiece,boardIndex"
        // Stored in order so MoveHistory can reconstruct the stack exactly.
        public List<string> UndoMoves { get; set; } = new();
        // The redo stack serialised in the same format as UndoMoves.
        // Preserves any undone moves so redo still works after loading.
        public List<string> RedoMoves { get; set; } = new();
	}
}
