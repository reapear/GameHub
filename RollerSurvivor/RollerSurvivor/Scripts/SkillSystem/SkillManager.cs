using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RollerSurvivor.Scripts.Framework;

namespace RollerSurvivor.Scripts.SkillSystem
{
    public class SkillManager : Singleton<SkillManager>
    {
        private List<Skill> m_SkillPoll;
        public void FireSkill(Vector2 Position,SkillFaction SkillFaction)
        {
            // // 从对象池获取或创建新技能
            // Skill skill = GetAvailableSkill();
            //
            // // 初始化技能属性
            // skill.Position = Position;
            // skill.Direction = Vector2.Normalize(player.Position - Position);
            // skill.IsActive = true;
            //
            // // 添加粒子效果
            // for (int i = 0; i < 10; i++)
            // {
            //     Particle particle = new Particle
            //     {
            //         Position = Position,
            //         Velocity = new Vector2(
            //             Raylib.GetRandomValue(-100, 100) / 100f,
            //             Raylib.GetRandomValue(-100, 100) / 100f),
            //         LifeTime = 1.0f,
            //         Size = 5.0f,
            //         Color = skill.EffectColor
            //     };
            //     skill.Particles.Add(particle);
            // }
        }
    }
}