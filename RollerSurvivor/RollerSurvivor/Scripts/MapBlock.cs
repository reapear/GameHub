using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RollerSurvivor.Scripts
{
    [Serializable]
    public class MapBlock
    {
        public int Width { get; set; }
        public int Height { get; set; }
        
        [JsonConverter(typeof(BoolArray2DConverter))]
        public bool[,] Map { get; set; }

        public MapBlock(int width,int height)
        {
            Width = width;
            Height = height;
            Map = new bool[Width, Height];
        }
        public MapBlock(){}

        public MapBlock(string fullPath)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                string jsonContent = File.ReadAllText(fullPath);
                var mapBlock = JsonSerializer.Deserialize<MapBlock>(jsonContent,options);
                Width = mapBlock.Width;
                Height = mapBlock.Height;
                Map = new bool[Width, Height];
                for (int i = 0; i < Width; i++)
                {
                    for (int j = 0; j < Height; j++)
                    {
                        Map[i, j] = mapBlock.Map[i, j];
                    }
                }
            }
            catch (Exception ex)
            {
                // 处理异常
                Console.WriteLine($"加载文件失败：{ex.Message}");
            }
        }
    }

    public class ScenceMap
    {
        public MapBlock MapBlock;
        public ScenceMap(int width, int height)
        {
            MapBlock = new MapBlock(width, height);
        }

        public ScenceMap(MapBlock mapBlock)
        {
            MapBlock = mapBlock;
        }
    }
}