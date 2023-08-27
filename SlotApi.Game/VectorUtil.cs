using System.Runtime.Serialization;

namespace SlotApi.Game
{
  public struct Vector2I 
  {
    public int X { get; }
    public int Y { get; }

    public Vector2I(int x, int y)
    {
      this.Y = y;
      this.X = x;
    }

    public int[] Ary { get => new [] { X, Y }; }

    public int LengthSq()
    {
      return this.X * this.X + this.Y * this.Y;
    }

    public static Vector2I operator -(Vector2I a, Vector2I b) => new Vector2I(a.X - b.X, a.Y - b.Y);
  }
}
