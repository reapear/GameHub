using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using RollerSurvivor.Scripts.Framework;
using RollerSurvivor.Scripts.PalyerController;

namespace RollerSurvivor.Scripts
{
    public class Player : ComponentGameEntity
    {

        private Rectangle _rectangle;
        public MaxHealthComponent MaxHealthComponent { get; set; }

        public JumpComponent JumpComponent;

        public PlayerStateMachine StateMachine;

        public Dictionary<State, PlayerState> StatePool;

        public float Speed = 100;
        public Player(float x, float y, Color color, int maxHealth) 
            : base(x, y, color)
        {
            InitializeComponents(maxHealth);
            
            _rectangle = new Rectangle(Position,10,10);

            StatePool = new Dictionary<State, PlayerState>()
            {
                {State.Move, new MoveState(this)},
                {State.JumpUp, new JumpUpState(this)},
                {State.JumpDown, new JumpDownState(this)},
            };

            JumpComponent = new VerticalAscentSubduction()
            {
                JumpMaxTime = 2f,
                SubductionSpeed = 500f
            };
            StateMachine = new PlayerStateMachine();
            StateMachine.Initialize(StatePool[State.Move]);
        }

        public override void Update()
        {
            StateMachine.Update();
            base.Update();
        }

        public override void Draw()
        {
            _rectangle.Width = _rectangle.Height = 10 * (1 + JumpComponent.ZPos);
            _rectangle.Position = Position;
            Raylib.DrawRectanglePro(_rectangle, Vector2.Zero, 0, EntityColor);
        }

        /// <summary>
        /// 改变状态
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(State state)
        {
            if (StatePool.TryGetValue(state, out var playerState))
            {
                StateMachine.ChangeState(playerState);
            }
            else
            {
                Console.WriteLine("未知状态");
            }
        }


        /// <summary>
        /// 初始化组件
        /// </summary>
        /// <param name="maxHealth"></param>
        private void InitializeComponents(int maxHealth)
        {
            MaxHealthComponent = new MaxHealthComponent(maxHealth, maxHealth);
        }
    }
}