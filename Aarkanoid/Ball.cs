using System;
using AEngine.Shapes;
using OpenTK;

namespace Aarkanoid
{
    internal class Ball : Cuboid
    {
        private bool activated;

        public Ball(Player owner, Vector3 min, Vector3 max) : base(min, max)
        {
            Owner = owner;
        }

        public Player Owner { get; set; }

        public Vector3 Movement { get; set; }

        public bool Activated
        {
            get { return activated; }
            set
            {
                activated = value;
                if (activated)
                    Timer.Set("activated", 0.33f);
            }
        }

        public override void Start()
        {
            base.Start();

            AddHitBox("mass", Min, Max);
        }

        public override void Update()
        {
            base.Update();

            Move();

            ManageCollisions();
        }

        private void Move()
        {
            LastPosition = Position;

            if (Activated)
                Position = new Vector3(
                    Position.X + Movement.X * Speed * DeltaTime,
                    Position.Y + Movement.Y * Speed * DeltaTime,
                    Position.Z + Movement.Z * Speed * DeltaTime);
            else
                Position = new Vector3(
                    Owner.Position.X + Owner.Min.X + Owner.Max.X / 2 - Max.X / 2,
                    Owner.Position.Y + Owner.Min.Y - Max.Y,
                    Owner.Position.Z + Owner.Min.Z + Owner.Max.Z / 2 - Max.Z / 2);
        }

        private void ManageCollisions()
        {
            if (Activated && Timer.Get("activated") <= 0)
            {
                var collisions = CheckCollisions();
                foreach (var collision in collisions)
                {
                    bool bounce = false;
                    var block = collision.Other as Block;
                    if (collision.Other is Player)
                    {
                        bounce = true;
                        var angle = 0.8f*(
                            Math.PI + -1f *
                            Math.PI * ((Min.X + Position.X - collision.Other.Position.X)/collision.OtherHitBox.Max.X));
                        Movement = new Vector3((float) Math.Cos(angle), (float) -Math.Sin(angle), Movement.Z).Normalized();
                    }
                    else if (collision.OtherHitBox.Name == "wallTop" || collision.OtherHitBox.Name == "wallBottom")
                    {
                        bounce = true;
                        Movement = new Vector3(Movement.X, -Movement.Y, Movement.Z);
                    }
                    else if (collision.OtherHitBox.Name == "wallLeft" || collision.OtherHitBox.Name == "wallRight")
                    {
                        bounce = true;
                        Movement = new Vector3(-Movement.X, Movement.Y, Movement.Z);
                    }
                    else if (block != null)// && !block.Falling)
                    {
                        bounce = true;
                        var mt2d = collision.MinimumTranslation2D;
                        Movement = new Vector3(
                            Movement.X * (mt2d.X != 0 ? -1 : 1), 
                            Movement.Y * (mt2d.Y != 0 ? -1 : 1),
                            Movement.Z);
                        block.Fall();
                    }

                    if (bounce)
                    {
                        Position = LastPosition;
                        Move();
                    }
                }
            }
        }

        public Vector3 LastPosition { get; private set; }
        public float Speed { get; set; }
    }
}