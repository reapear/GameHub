using Raylib_cs;
using RollerSurvivor.Scripts.Framework;

public class Monster : GameEntity
{
    public MaxHealthComponent MaxHealthComponent { get; set; }
    public DamageComponent DamageComponent { get; set; }

    public Monster(int x, int y, Color color, int maxHealth, int attackPower) 
        : base(x, y, color)
    {
        InitializeComponents(maxHealth, attackPower);
    }

    private void InitializeComponents(int maxHealth, int attackPower)
    {
        MaxHealthComponent = new MaxHealthComponent(maxHealth, maxHealth);
        DamageComponent = new DamageComponent(attackPower);
    }

    // 可以在这里添加怪物特有的方法
}