namespace Battleships.Models.DataTypes
{
    /// <summary>
    /// Represents a two-dimensional vector with integer components.
    /// </summary>
    /// <param name="x">X coordinate</param>
    /// <param name="y">Y coordinate</param>
    public struct Vector2(int x, int y) : IEquatable<Vector2>
    {
        /// <summary>
        /// Gets or sets the X coordinate of the vector.
        /// </summary>
        public int X { get; set; } = x;

        /// <summary>
        /// Gets or sets the Y coordinate of the vector.
        /// </summary>
        public int Y { get; set; } = y;

        public override string ToString()
        {
            return $"{X},{Y}";
        }

        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }
    }
}
