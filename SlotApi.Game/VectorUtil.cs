namespace SlotApi.Game
{
  public struct Vector2I
  {
    public int X { get; private set; }
    public int Y { get; private set; }

    public Vector2I(int x, int y)
    {
      this.Y = y;
      this.X = x;
    }

    public int LengthSq()
    {
      return this.X * this.X + this.Y * this.Y;
    }

    public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.X - b.X, a.Y - b.Y);
  }

  //public class Vector2
  //{
  //  private readonly Vector<int> inner;

  //  public Vector2(int xSize, int ySize)
  //  {
  //    this.inner = new Vector<int>(new[] { xSize, ySize });
  //  }

  //  public int X
  //  {
  //    get => this.inner[0];
  //  }

  //  public int Y
  //  {
  //    get => this.inner[1];
  //  }
  //}
}
