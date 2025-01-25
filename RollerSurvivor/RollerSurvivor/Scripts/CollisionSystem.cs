using System.Collections.Generic;
using RollerSurvivor.Scripts.Framework;

namespace RollerSurvivor.Scripts
{
    public class CollisionSystem : Singleton<CollisionSystem>
    {
        private List<CollisionComponent> CollisionComponents = new List<CollisionComponent>();

        public bool[,] CollisionMask = new bool[,]
        {
            {true, false, false},
            {false, true, false},
            {false, false, true}
        };
        
        public void Register(CollisionComponent component)
        {
            if (!CollisionComponents.Contains(component))
            {
                CollisionComponents.Add(component);
            }
        }

        // 注销碰撞组件
        public void Unregister(CollisionComponent component)
        {
            if (CollisionComponents.Contains(component))
            {
                CollisionComponents.Remove(component);
            }
        }

        // 每帧更新碰撞检测
        public void UpdateCollisions()
        {
            for (int i = 0; i < CollisionComponents.Count; i++)
            {
                for (int j = i + 1; j < CollisionComponents.Count; j++)
                {
                    var a = CollisionComponents[i];
                    var b = CollisionComponents[j];
                    if (CollisionMask[a.Layer, b.Layer])
                    {
                        a.CheckCollision(b);
                    }
                }
            }
        }
    }
}