[System.Serializable]
public struct SGrid : System.IEquatable<SGrid>
{
    public int x, y;
    public SGrid(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public SGrid Shift(int x, int y) => new(this.x + x, this.y + y);
    public SGrid Shift(SGrid sGrid) => Shift(sGrid.x, sGrid.y);
    public bool Equals(SGrid other) => x == other.x && y == other.y;
    public override bool Equals(object obj) => obj is SGrid other && Equals(other);
    public override int GetHashCode() => System.HashCode.Combine(x, y);
}