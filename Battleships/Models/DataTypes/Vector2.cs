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

        /// <summary>
        /// Returns a string representation of the current object.
        /// </summary>
        /// <returns>A string in the format "X,Y", where X and Y represent the values of the respective properties.</returns>
        public override string ToString()
        {
            return $"{X},{Y}";
        }

        /// <summary>
        /// Determines whether the current <see cref="Vector2"/> instance is equal to the specified <see
        /// cref="Vector2"/> instance.
        /// </summary>
        /// <param name="other">The <see cref="Vector2"/> instance to compare with the current instance.</param>
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }

        /// <summary>
        /// Determines whether the specified object is equal to the current instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current instance..</param>
        public override bool Equals(object? obj)
        {
            return obj is Vector2 other && Equals(other);
        }

        /// <summary>
        /// Returns a hash code for the current object.
        /// </summary>
        /// <returns>An integer representing the hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        /// <summary>
        /// Determines whether two <see cref="Vector2"/> instances are equal.
        /// </summary>
        /// <param name="left">The first <see cref="Vector2"/> instance to compare.</param>
        /// <param name="right">The second <see cref="Vector2"/> instance to compare.</param>
        public static bool operator ==(Vector2 left, Vector2 right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="Vector2"/> instances are not equal.
        /// </summary>
        /// <param name="left">The first <see cref="Vector2"/> instance to compare.</param>
        /// <param name="right">The second <see cref="Vector2"/> instance to compare.</param>
        public static bool operator !=(Vector2 left, Vector2 right)
        {
            return !(left == right);
        }
    }
}
