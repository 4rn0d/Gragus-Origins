using UnityEngine;

namespace Map
{
    public enum Direction
    {
        North,
        South,
        East,
        West
    }

    public static class DirectionExtensions
    {
        public static Direction Opposite(this Direction dir)
        {
            return dir switch
            {
                Direction.North => Direction.South,
                Direction.South => Direction.North,
                Direction.East => Direction.West,
                Direction.West => Direction.East,
                _ => dir,
            };
        }

        public static Vector2 ToVector2(this Direction dir)
        {
            return dir switch
            {
                Direction.North => Vector2.up,
                Direction.South => Vector2.down,
                Direction.East => Vector2.right,
                Direction.West => Vector2.left,
                _ => Vector2.zero
            };
        }
    }

}