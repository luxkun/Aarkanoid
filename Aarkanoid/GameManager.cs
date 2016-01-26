using System;
using System.Collections.Generic;
using AEngine;
using AEngine.Shapes;
using OpenTK;
using OpenTK.Graphics;

namespace Aarkanoid
{
    public class GameManager : GameObject
    {
        private static GameManager instance;
        private static Random random;
        public static GameManager I => instance ?? (instance = new GameManager());
        public static Random Random => random ?? (random = new Random());

        private GameManager()
        {
        }

        public override void Start()
        {
            base.Start();
            var player = new Player(Vector3.Zero, new Vector3(40f, 10f, 10f));
            player.Position = new Vector3(-20f, 160f, 0);

            Engine.SpawnObject("player", player);

            Engine.Camera.Position = new Vector3(0f, 0f, -400f);
            Engine.Camera.Type = Camera.ProjectionType.Prospective;

            AddLevelWalls();

            LoadLevels();
        }

        private void LoadLevels()
        {
            Levels = new List<Level>();

            var level0 = new Level
            {
                BlockSize = new Vector3(15f, 6f, 8f)
            };
            var padding = 10f;
            level0.BlockPadding = padding;
            var blockSpace = 200f;
            level0.SpawnRandomBlocks(
                140, 
                new Vector2(TopLeftBound.X + padding*2, TopLeftBound.Y + padding * 2),
                new Vector2(TopRightBound.X - padding * 2, TopRightBound.Y + padding * 2 + blockSpace),
                0f);

            Engine.SpawnObject("level0", level0);
            Levels.Add(level0);
        }

        public List<Level> Levels { get; private set; }

        private void AddLevelWalls()
        {
            var hitBoxExtraSize = new Vector3(0f, 0f, 100f);
            var wallTop = new Cuboid(Vector3.Zero, new Vector3(400f, 20f, 20f))
            {
                Position = TopLeftBound, 
                Color = Color4.Red
            };
            wallTop.AddHitBox("wallTop", wallTop.Min - hitBoxExtraSize, wallTop.Max + hitBoxExtraSize);

            var wallLeft = new Cuboid(Vector3.Zero, new Vector3(20f, 400f, 20f))
            {
                Position = TopLeftBound,
                Color = Color4.AliceBlue
            };
            wallLeft.AddHitBox("wallLeft", wallLeft.Min - hitBoxExtraSize, wallLeft.Max + hitBoxExtraSize);

            var wallRight = new Cuboid(Vector3.Zero, new Vector3(20f, 400f, 20f))
            {
                Position = TopRightBound,
                Color = Color4.AliceBlue
            };
            wallRight.AddHitBox(
                "wallRight", wallRight.Min - hitBoxExtraSize, wallRight.Max + hitBoxExtraSize);

            var wallBottom = new Cuboid(Vector3.Zero, new Vector3(400f, 20f, 20f))
            {
                Position = BottomLeftBound,
                Color = Color4.AliceBlue
            };
            wallBottom.AddHitBox(
                "wallBottom", wallBottom.Min - hitBoxExtraSize, wallBottom.Max + hitBoxExtraSize);

            Engine.SpawnObject("wallTop", wallTop);
            Engine.SpawnObject("wallLeft", wallLeft);
            Engine.SpawnObject("wallRight", wallRight);
            Engine.SpawnObject("wallBottom", wallBottom);
        }

        public Vector3 BottomRightBound { get; set; } = new Vector3(200f, 200f, 0);
        public Vector3 BottomLeftBound { get; set; } = new Vector3(-200f, 200f, 0);
        public Vector3 TopRightBound { get; set; } = new Vector3(200f, -200f, 0);
        public Vector3 TopLeftBound { get; set; } = new Vector3(-200f, -200f, 0);
    }
}