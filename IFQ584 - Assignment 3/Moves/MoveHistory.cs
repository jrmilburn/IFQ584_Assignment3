//Contains history of move records
class MoveHistory
{
    private Stack<MoveRecord> doneMoves;
    private Stack<MoveRecord> undoneMoves;
    private int currentMoveIndex;
    public MoveHistory()
    {
        doneMoves = new Stack<MoveRecord>();
		undoneMoves = new Stack<MoveRecord>();
		currentMoveIndex = 0;
    }
    public int CurrentMoveIndex => currentMoveIndex;
    public void Add(MoveRecord record)
	{
		doneMoves.Push(record);
		//Clear redo options once move is made
		ClearRedo();
		currentMoveIndex = doneMoves.Count;
	}
    public MoveRecord Undo()
    {
		bool validUndo = CanUndo();

		if (validUndo)
		{
			MoveRecord undoneMove = doneMoves.Pop();
			undoneMoves.Push(undoneMove);
			currentMoveIndex -=1;
			return undoneMove;
		}

		return null;
    }

    public MoveRecord Redo()
    {
		bool validRedo = CanRedo();

		if (validRedo)
		{
			MoveRecord redoneMove = undoneMoves.Pop();
			doneMoves.Push(redoneMove);
			currentMoveIndex += 1;
			return redoneMove;
		}

		return null;
    }

    //When a move is undone and then a different move is played,
    // the remainder of the stack should be cleared
    public void ClearRedo()
    {
        undoneMoves.Clear();
    }

    public bool CanUndo()
    {
        //If a move has been performed, undo becomes available
        if(doneMoves.Count > 0)
        {
            return true;
        }

        return false;
    }

    public bool CanRedo()
    {
		//If a move has been undo then redo becomes available
		if(undoneMoves.Count > 0)
		{
			return true;
		}

        return false;
    }
}