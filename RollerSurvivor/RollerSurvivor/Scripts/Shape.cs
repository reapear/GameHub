using System;
using System.Numerics;
using Raylib_cs;

namespace RollerSurvivor.Scripts
{
    public class Shape
    {
        public virtual int ShapeType { get; }
        public Vector2 Position;
        public float[] Param;
        
        public bool CheckCollision(Shape other)
        {
            switch (ShapeType,other.ShapeType)
            {
                case (1,1):
                    return Raylib.CheckCollisionCircles(Position, Param[0], other.Position, other.Param[0]);
                default:
                    Console.WriteLine($"未知形状{ShapeType} {other.ShapeType}");
                    return false;
            }
        }
    }
}