using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace RollerSurvivor.Scripts.SkillSystem
{
    public class Skill
    {
        // 基础属性
        public SkillFaction SkillFaction;
        public Vector2 Position;
        public Vector2 Direction;
        public float Speed = 500f;
        public float Radius = 20f;    // 作用范围
        public int Damage = 30;       // 伤害值
        public bool IsActive = false; // 是否激活

        // 特效参数
        public Color EffectColor = Color.Blue;
        public List<Particle> Particles = new List<Particle>();
    }
}