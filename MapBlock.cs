using System;

namespace RollerSurvivor.Scripts
{
    [Serializable]
    public class MapBlock
    {
        public int Width;
        public int Height;
        public bool[,] Map;

        public MapBlock(int width,int height)
        {
            Width = width;
            Height = height;
            Map = new bool[Width, Height];
        }
    }
}