//Contains history of move records

class MoveHistory
{
    private Stack<MoveRecord> doneMoves;
    private Stack<MoveRecord> undoneMoves;
    private int currentMoveIndex;
    public MoveHistory()
    {
        
    }
    public int CurrentMoveIndex => currentMoveIndex;
    public void Add(MoveRecord record){
    }
    public void Undo()
    {
        currentMoveIndex -= 1;
    }

    public void Redo()
    {
        currentMoveIndex += 1;
    }

    //When a move is undone and then a different move is played,
    // the remainder of the stack should be cleared
    public void ClearRedo()
    {
        for(int i = 0; i < doneMoves.Count(); ++i)
        {
            if(i > currentMoveIndex)
            {
                //Implement clearing logic
            }
        }
    }

    public bool CanUndo()
    {
        //If a move has been performed, undo becomes available
        if(currentMoveIndex > 0)
        {
            return true;
        }

        return false;
    }

    public bool CanRedo()
    {
        //Redo becomes available once a move has been undone
        //In practice this means the stack length is greater than the current index
        int stackLength = doneMoves.Count();
        if(currentMoveIndex < stackLength)
        {
            return true;
        }

        return false;
    }
}