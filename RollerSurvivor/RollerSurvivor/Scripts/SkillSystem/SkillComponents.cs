
using System.Numerics;

namespace RollerSurvivor.Scripts.SkillSystem
{
    #region 投射物

    public class ProjectileComponent : RollerSurvivor.Scripts.Framework.Component
    {
        public virtual void OnEnter()
        {
            
        }
    }

    public class DirectionProjectileComponent : ProjectileComponent
    {
        public Vector2 Direction;
        public Vector2 Speed;
    }

    public class PositionTargetProjectileComponent : ProjectileComponent
    {
        
    }

    public class EntityTargetProjectileComponent : ProjectileComponent
    {
        
    }

    public class StaticProjectile : RollerSurvivor.Scripts.Framework.Component
    {
    }

    #endregion

}