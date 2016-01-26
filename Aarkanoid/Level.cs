using System;
using System.Collections.Generic;
using AEngine;
using AEngine.Shapes;
using OpenTK;
using OpenTK.Graphics;

namespace Aarkanoid
{
    public class Level : GameObject
    {
        public List<Block> Blocks { get; set; } 
        public Vector3 BlockSize { get; set; }
        public float BlockPadding { get; set; }

        public Level() : base()
        {
            Blocks = new List<Block>();
        }

        public void SpawnRandomBlocks(int number, Vector2 start, Vector2 end, float z)
        {
            var position = new Vector3(start.X, start.Y, z);
            for (int index = 0; index < number; index++)
            {
                var block = new Block(Vector3.Zero, BlockSize)
                {
                    Position = position,
                    Color = BlockColor
                };
                position.X += BlockSize.X + BlockPadding;
                if (position.X > end.X) { 
                    position.X = start.X;
                    position.Y += BlockSize.Y + BlockPadding;
                }
                Blocks.Add(block);
            }
        }

        public Color4 BlockColor { get; set; } = Color4.Blue;

        public override void Start()
        {
            base.Start();
            for (int index = 0; index < Blocks.Count; index++)
            {
                var block = Blocks[index];
                Engine.SpawnObject($"{Name}_block{index}", block);
            }
        }
    }

    public class Block : Cuboid
    {
        private Vector3 startPosition;
        private float zTilt = 10f;
        private float zTiltSpeed = 5f;
        private bool tiltSign;
        public bool Falling { get; set; }
        private readonly float fallingSpeed = 60f;
        private readonly float fallingRotation = (float) Math.PI;

        public Block(Vector3 min, Vector3 max) : base(min, max)
        {
        }

        public override void Start()
        {
            base.Start();
            startPosition = Position;
            zTilt *= (float)GameManager.Random.NextDouble() + 0.5f;
            zTiltSpeed *= (float)GameManager.Random.NextDouble() + 0.5f;
            tiltSign = GameManager.Random.Next(2) == 0;

            AddHitBox("mass", Min - new Vector3(0f, 0f, 20f), Max + new Vector3(0f, 0f, 20f));
        }

        public override void Update()
        {
            base.Update();
            if (Falling)
            {
                if (Timer.Get("destroy") <= 0)
                    Destroy();
                else
                {
                    Position = new Vector3(Position.X, Position.Y + fallingSpeed * DeltaTime, Position.Z);
                    Rotation = new Vector3(Rotation.X, Rotation.Y + fallingRotation * DeltaTime, Rotation.Z);
                }
            } else
            { 
                var tiltMod = zTiltSpeed*DeltaTime*(tiltSign ? 1 : -1);
                if ((tiltSign && Position.Z > startPosition.Z + zTilt) ||
                    (!tiltSign && Position.Z < startPosition.Z - zTilt))
                    tiltSign = !tiltSign;
                Position = new Vector3(Position.X, Position.Y, Position.Z + tiltMod);
            }
        }


        public void Fall()
        {
            RemoveAllHitBoxes();
            Timer.Set("destroy", 8f);
            Falling = true;
        }
    }
}