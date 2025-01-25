using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace RollerSurvivor.Scripts.Framework
{
    public class Component
    {
        public ComponentGameEntity Entity { get; private set; }

        public void BindEntity(ComponentGameEntity componentGameEntity)
        {
            Entity = componentGameEntity;
        }

        public virtual void Update()
        {
            
        }
    }

    public class CollisionComponent : Component
    {
        public Shape Collision;

        public int Layer;

        public List<CollisionComponent> Stays = new List<CollisionComponent>();

        protected virtual void OnEnter(CollisionComponent other)
        {
            // 处理刚进入碰撞时的逻辑
        }

        protected virtual void OnStay(CollisionComponent other)
        {
            // 处理持续碰撞时的逻辑
        }

        protected virtual void OnExit(CollisionComponent other)
        {
            // 处理退出碰撞时的逻辑
        }

        public void CheckCollision(CollisionComponent other)
        {
            if (Collision.CheckCollision(other.Collision))
            {
                if (Stays.Contains(other) || other.Stays.Contains(this))
                {
                    OnStay(other);
                    other.OnStay(this);
                }
                else
                {
                    Stays.Add(other);
                    OnEnter(other);
                    other.OnEnter(this);
                }
            }
            else
            {
                if (Stays.Remove(other))
                {
                    other.Stays.Remove(this);
                }
                OnExit(other);
                other.OnExit(this);
            }
        }
    }
    
    public class HealthComponent : Component
    {
        public int Health { get; set; }
        public bool IsDead => Health <= 0;
    
        public HealthComponent(int health)
        {
            Health = health;
        }
        
        public virtual void TakeDamage(int damage)
        {
            Health -= damage;
        }
    
        public virtual void Heal(int amount)
        {
            Health += amount;
        }
    }
    
    public class MaxHealthComponent : HealthComponent
    {
        public int MaxHealth{ get; set; }
        public MaxHealthComponent(int health, int maxHealth) : base(health)
        {
            MaxHealth = maxHealth;
        }
    
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }
    
        public override void Heal(int amount)
        {
            if(IsDead)
            {
                return;
            }
            base.Heal(amount);
            Health = Math.Min(Health, MaxHealth);
        }
    }
    
    public class DamageComponent : CollisionComponent
    {
        public int Attack { get; set; }
        public int ElementalAttack { get; set; }
    
        public DamageComponent(int attack, int elementalAttack = 0)
        {
            Attack = attack;
            ElementalAttack = elementalAttack;
        }
    
        public virtual void IncreaseAttack(int amount)
        {
            Attack += amount;
        }
    
        public virtual void IncreaseElementalAttack(int amount)
        {
            ElementalAttack += amount;
        }
    }
    
    public class JumpComponent : Component
    {
        public float JumpMaxTime;

        public float ZPos;
    }

    public class VerticalAscentSubduction : JumpComponent
    {
        public float SubductionSpeed;
    }
}