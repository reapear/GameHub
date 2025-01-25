 using System;
using System.Collections.Generic;
using Raylib_cs;
 using RollerSurvivor.Scripts;
 using Color = Raylib_cs.Color;

public class EnemySpawner
{
    private float spawnTimer = 0;
    private float spawnInterval = 1.0f; // 每秒生成一个敌人
    private float innerRadius = 300f; // 内圆半径
    private float outerRadius = 500f; // 外圆半径
    private Player player;
    private List<GameEntity> gameEntities;
    private Random random;

    public EnemySpawner(Player player, List<GameEntity> gameEntities)
    {
        this.player = player;
        this.gameEntities = gameEntities;
        this.random = new Random();
    }

    public void Update(float deltaTime)
    {
        spawnTimer += deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0;
        }
    }

    private void SpawnEnemy()
    {
        // 生成随机角度
        float angle = (float)(random.NextDouble() * Math.PI * 2);
        // 生成随机半径（在内外圆之间）
        float radius = innerRadius + (float)(random.NextDouble() * (outerRadius - innerRadius));
        
        // 计算生成位置
        // float spawnX = player.X + radius * (float)Math.Cos(angle);
        // float spawnY = player.Y + radius * (float)Math.Sin(angle);
        
    }
}