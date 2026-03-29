
public interface IBoard
{
    bool IsEmpty(int x, int y);
    string GetCell(int x, int y);
    void SetCell(int x, int y, string value);
    IBoard Clone();
    string Serialise();
    void Render();
}
