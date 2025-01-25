using System.Numerics;
using RollerSurvivor.Scripts.Framework;

namespace RollerSurvivor.Scripts
{
    public class MapManager : Singleton<MapManager>
    {
        public ScenceMap CurrentScenceMap { get; private set; }

        public readonly int Tilesize = 100;

        public MapManager()
        {
            CurrentScenceMap = new ScenceMap(new MapBlock("E:/MaskMapAsset/Test.json"));
        }

        public bool CanMove(Vector2 position)
        {
            var x = (int)(position.X/100);
            var y = (int)(position.Y/100);
            if (x >= 0 && y >= 0 && x < CurrentScenceMap.MapBlock.Width && y < CurrentScenceMap.MapBlock.Height)
            {
                return CurrentScenceMap.MapBlock.Map[x, y];
            }
            return true;
        }
    }
}