using System;
using System.Numerics;
using Raylib_cs;
using RollerSurvivor.Scripts.Framework;

namespace RollerSurvivor.Scripts.PalyerController
{
    public abstract class PlayerState
    {
        protected Player player;
        
        public PlayerState(Player player) => this.player = player;
    
        public virtual void Enter() {}  // 进入状态时调用
        public virtual void Exit() {}   // 退出状态时调用
        public virtual void Update() {} // 每帧更新
        public virtual void FixedUpdate() {} // 物理更新
    }
    
    public class PlayerStateMachine
    {
        private PlayerState currentState;

        public void Initialize(PlayerState startingState)
        {
            currentState = startingState;
            currentState.Enter();
        }

        public void ChangeState(PlayerState newState)
        {
            Console.WriteLine($"切换状态{newState}");
            currentState.Exit();
            currentState = newState;
            currentState.Enter();
        }

        public void Update() => currentState.Update();
        // public void FixedUpdate() => currentState.FixedUpdate();
    }
    
    // public class IdleState : PlayerState
    // {
    //     public IdleState(Player player) : base(player) {}
    //
    //     public override void Update()
    //     {
    //         // 检测输入切换到移动状态
    //         if (player.MoveInput != Vector2.Zero)
    //         {
    //             player.StateMachine.ChangeState(new MoveState(player));
    //         }
    //
    //         // 检测跳跃输入
    //         if (player.JumpInput)
    //         {
    //             player.StateMachine.ChangeState(new JumpState(player));
    //         }
    //     }
    // }
    
    public class MoveState : PlayerState
    {
        public MoveState(Player player) : base(player) {}

        public override void Enter()
        {
            // player.Animator.Play("Run"); // 播放奔跑动画
        }

        public override void Update()
        {
            if (Raylib.GetMouseWheelMove() > 0)
            {
                if (player.JumpComponent is VerticalAscentSubduction)
                {
                    player.ChangeState(State.JumpUp);
                }
                return;
            }
            var mousePosition = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), CameraManager.Instance.Camera);
            var newPosition = Vector2.Lerp(player.Position , mousePosition, player.Speed * Raylib.GetFrameTime() / Vector2.Distance(player.Position, mousePosition));
            if (MapManager.Instance.CanMove(newPosition))
            {
                player.Position = newPosition;
            }
        }
    }
    
    public class JumpUpState : PlayerState
    {
        private float jumpTimer;

        private float jumpMaxTime;

        public JumpUpState(Player player) : base(player) {}

        public override void Enter()
        {
            // player.Animator.Play("Jump");
            // player.Rigidbody2D.AddForce(Vector2.up * player.JumpForce, ForceMode2D.Impulse);
            jumpTimer = 0;
            jumpMaxTime = player.JumpComponent.JumpMaxTime;
        }

        public override void Update()
        {
            var mouseWheelMove = Raylib.GetMouseWheelMove();
            if (jumpTimer < 1f || jumpTimer < jumpMaxTime && mouseWheelMove >0)
            {
                jumpTimer += Raylib.GetFrameTime();
                player.JumpComponent.ZPos += Raylib.GetFrameTime();
            }
            else
            {
                player.ChangeState(State.JumpDown);
            }
        }
    }
    
    public class JumpDownState : PlayerState
    {
        private float speed;

        private Vector2 targetPos;

        public JumpDownState(Player player) : base(player) {}

        public override void Enter()
        {
            speed = (player.JumpComponent as VerticalAscentSubduction).SubductionSpeed;
            Console.WriteLine($"speed:{speed}");
            targetPos = Raylib.GetScreenToWorld2D(Raylib.GetMousePosition(), CameraManager.Instance.Camera);
        }

        public override void Update()
        {
            var distance = Vector2.Distance(player.Position, targetPos);
            if (distance > 10f)
            {
                var duration = speed * Raylib.GetFrameTime() / Vector2.Distance(player.Position, targetPos);
                player.Position = Vector2.Lerp(player.Position , targetPos, duration);
                player.JumpComponent.ZPos = MathF.Max(player.JumpComponent.ZPos * (1 - duration),0);
            }
            else
            {
                player.Position = targetPos;
                player.JumpComponent.ZPos = 0;
                player.ChangeState(State.Move);
            }
        }
    }
}