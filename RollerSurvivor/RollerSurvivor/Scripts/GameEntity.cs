using System.Collections.Generic;
using System.ComponentModel;
using System.Numerics;
using Raylib_cs;
using Fw = RollerSurvivor.Scripts.Framework;

public class GameEntity
{
    public Vector2 Position;
    public Color EntityColor { get; set; }

    public GameEntity(float x, float y, Color color)
    {
        Position = new Vector2(x, y);
        EntityColor = color;
    }

    public virtual void Draw()
    {
    }

    public virtual void Update()
    {
        // 更新逻辑可以在子类中实现
    }
} 

public class ComponentGameEntity : GameEntity
{
    public List<Fw.Component> Components { get; set; }
    public ComponentGameEntity(float x, float y, Color color) 
        : base(x, y, color)
    {
        Components = new List<Fw.Component>();
    }

    public T GetComponent<T>() where T : Fw.Component
    {
        foreach (var component in Components)
        {
            if (component is T typedComponent)
            {
                return typedComponent;
            }
        }
        return null;
    }

    public override void Update()
    {
        base.Update();
        foreach (var component in Components)
        {
            component.Update();
        }
    }

    public void AddComponent(Fw.Component component)
    {
        Components.Add(component);
        component.BindEntity(this);
    }

    public void RemoveComponent(Fw.Component component)
    {
        Components.Remove(component);
    }
}