using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Raylib_cs;
using RollerSurvivor.Scripts;
using Color = Raylib_cs.Color;

public class SurvivorGame
{
    #region BasicField
    
    private Player Player;
    
    private List<GameEntity> GameEntitys;

    private int _tilesize;

    private CollisionSystem _collisionSystem = CollisionSystem.Instance;

    #endregion

    public SurvivorGame()
    {
        Init();
        Raylib.InitWindow(800, 600, "Survivor Game"); // 初始化窗口
        Raylib.SetTargetFPS(60); // 设置帧率
    }

    public void Init()
    {
        Player = new Player(400, 300, Color.Green, 100);
        GameEntitys = new List<GameEntity>();

        _tilesize = MapManager.Instance.Tilesize;

        // 添加其他实体到 GameEntitys 列表中
    }

    public void Run()
    {
        while (!Raylib.WindowShouldClose())
        {
            Player.Update();
            
            // 更新碰撞
            _collisionSystem.UpdateCollisions();
            
            // 更新摄像机目标为玩家的位置
            CameraManager.Instance.Update(Player.Position);

            // 控制缩放
            // ControlZoom();

            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Gray);
            
            Raylib.BeginMode2D(CameraManager.Instance.Camera);

            var map = MapManager.Instance.CurrentScenceMap.MapBlock;
            // 绘制地图
            for (int y = 0; y < map.Width; y++)
            {
                for (int x = 0; x < map.Height; x++)
                {
                    // 根据 Map[y, x] 的值选择颜色
                    Color color = map.Map[y, x] ? Color.Gray : Color.Black;

                    // 绘制方块
                    Raylib.DrawRectangle(x * _tilesize, y * _tilesize, _tilesize, _tilesize, color);
                }
            }

            DrawGameEntity(); // 绘制游戏实体
            Raylib.EndMode2D();

            Raylib.EndDrawing();
        }
        Raylib.CloseWindow();
    }

    #region Function
    
    /// <summary>
    /// 绘制游戏实体
    /// </summary>
    public void DrawGameEntity()
    {
        Player.Draw();
        foreach (var entity in GameEntitys)
        {
            entity.Draw();
        }
    }

    /// <summary>
    /// 控制摄像机缩放
    /// </summary>
    private void ControlZoom()
    {
        float scroll = Raylib.GetMouseWheelMove();
        CameraManager.Instance.ModifyZoom(scroll * 0.1f);
    }

    #endregion
}

// 添加 Main 方法作为程序入口点
public static class Program
{
    public static void Main()
    {
        SurvivorGame game = new SurvivorGame();
        game.Run();
    }
}
