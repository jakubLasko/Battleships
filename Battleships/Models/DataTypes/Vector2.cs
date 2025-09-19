namespace Battleships.Models.DataTypes
{
    public struct Vector2(int x, int y)
    {
        public int X { get; set; } = x;
        public int Y { get; set; } = y;

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
