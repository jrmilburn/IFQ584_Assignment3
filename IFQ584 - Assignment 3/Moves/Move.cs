using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGames
{
	public class Move
    {
        public int PlayerId { get; }
        public int X { get; }
        public int Y { get; }
        public string ValueOrPiece { get; }
        public int BoardIndex { get; }

        public Move(int playerId, int x, int y, string valueOrPiece = "X", int boardIndex = 0)
        {
            PlayerId = playerId;
            X = x;
            Y = y;
            ValueOrPiece = valueOrPiece;
            BoardIndex = boardIndex;
        }

        public override string ToString() =>
            $"Player {PlayerId} → ({X},{Y}) [{ValueOrPiece}] board={BoardIndex}";
    }
}
