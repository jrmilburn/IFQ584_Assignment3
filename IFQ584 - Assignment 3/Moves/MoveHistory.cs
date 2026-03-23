using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGames
{
	public class MoveHistory
	{
		private readonly Stack<Move> _undo = new();
		private readonly Stack<Move> _redo = new();

		public int UndoCount => _undo.Count;
		public int RedoCount => _redo.Count;
		public Move? UndoTop => _undo.TryPeek(out var m) ? m : null;

		public void DoMove(Move m)
		{
			_undo.Push(m);
			_redo.Clear();          // new move clears redo
		}

		public Move? Undo()
		{
			if (_undo.Count == 0) return null;
			var m = _undo.Pop();
			_redo.Push(m);
			return m;
		}

		public Move? Redo()
		{
			if (_redo.Count == 0) return null;
			var m = _redo.Pop();
			_undo.Push(m);
			return m;
		}

		public void ClearRedo() => _redo.Clear();

		public List<Move> UndoMoves() => _undo.ToList();
		public List<Move> RedoMoves() => _redo.ToList();

		public void Restore(List<Move> undoMoves, List<Move> redoMoves)
		{
			_undo.Clear(); _redo.Clear();
			// Stacks push in reverse
			foreach (var m in Enumerable.Reverse(undoMoves)) _undo.Push(m);
			foreach (var m in Enumerable.Reverse(redoMoves)) _redo.Push(m);
		}
	}
}
