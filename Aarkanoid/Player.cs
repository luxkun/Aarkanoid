using System;
using AEngine;
using AEngine.Shapes;
using Aiv.Fast2D;
using OpenTK;
using OpenTK.Graphics;

namespace Aarkanoid
{
    public class Player : Cuboid
    {
        private int ballCount;

        public event ActivateKeyEventHandler OnActivateKey;

        public Stats Stats { get; }
        public Player(Vector3 min, Vector3 max) : base(min, max)
        {
            Color = Color4.Honeydew;
            Stats = new Stats()
            {
                Speed = 75f,
                BallSpeed = 100f
            };
        }

        private void AddStickyBall()
        {
            var ball = new Ball(this, Vector3.Zero, new Vector3(5f, 5f, 5f))
            {
                Activated = false,
                Color = Color4.AliceBlue,
                Speed = Stats.BallSpeed
            };

            OnActivateKey += (sender, args) =>
            {
                if (!ball.Activated)
                {
                    var cone = 0.4; // max 0.5
                    var angle = Math.PI*GameManager.Random.NextDouble()*(1-cone*2) + Math.PI*cone;
                    ball.Movement = new Vector3((float) Math.Cos(angle), (float) -Math.Sin(angle), 0f);
                    Console.WriteLine($"Random ball direction: {ball.Movement} (angle {angle}");
                    ball.Activated = true;
                }
            };

            Engine.SpawnObject($"ball{ballCount++}", ball);
        }

        public override void Start()
        {
            base.Start();

            AddStickyBall();
            AddHitBox("mass", Min, Max);
        }

        public override void Update()
        {
            base.Update();
            ManageInput();
        }

        private void ManageInput()
        {
            var lastPosition = new Vector3(Position);
            Vector2 movement = Vector2.Zero;
            //if (Engine.IsKeyDown(KeyCode.W))
            //    movement.Y -= 1f;
            //if (Engine.IsKeyDown(KeyCode.S))
            //    movement.Y += 1f;
            if (Engine.IsKeyDown(KeyCode.A))
                movement.X -= 1f;
            if (Engine.IsKeyDown(KeyCode.D))
                movement.X += 1f;
            movement *= Stats.Speed * DeltaTime;
            Position = new Vector3(Position.X + movement.X, Position.Y + movement.Y, Position.Z);
            if (HasCollisions((other, hitbox) => !(other is Ball)))
                Position = lastPosition;

            if (Engine.IsKeyDown(KeyCode.Space))
                ActivateKey(new ActivateKeyEventHandlerArgs());
        }

        protected virtual void ActivateKey(ActivateKeyEventHandlerArgs args)
        {
            OnActivateKey?.Invoke(this, args);
        }
    }

    public delegate void ActivateKeyEventHandler(object sender, ActivateKeyEventHandlerArgs args);

    public class ActivateKeyEventHandlerArgs
    {
    }

    public class Stats
    {
        public float Speed { get; set; }
        public float BallSpeed { get; set; }
    }
}