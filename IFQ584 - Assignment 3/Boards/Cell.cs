//The following class represents a cell on the board
//I have implemented this specifically to address lecturer feedback from Assignment 2

abstract class Cell
{
    public abstract bool IsEmpty();
    public abstract string GetDisplayText();
}

class EmptyCell : Cell
{
    public override bool IsEmpty()
    {
        return true;
    }

    public override string GetDisplayText()
    {
        return ".";
    }
}

class NumberCell : Cell
{
    private int value;

    public NumberCell(int newValue)
    {
        value = newValue;
    }

    public int GetValue()
    {
        return value;
    }

    public override bool IsEmpty()
    {
        return false;
    }

    public override string GetDisplayText()
    {
        return value.ToString();
    }
}

class PieceCell : Cell
{
    private PieceType piece;

    public PieceCell(PieceType newPiece)
    {
        piece = newPiece;
    }

    public PieceType GetPiece()
    {
        return piece;
    }

    public override bool IsEmpty()
    {
        return false;
    }

    public override string GetDisplayText()
    {
        if(piece == PieceType.X)
        {
            return "X";
        }

        if(piece == PieceType.O)
        {
            return "O";
        }

        if(piece == PieceType.Neutral)
        {
            return "X";
        }

        return "?";
    }
}