using System;
using System.Collections.Generic;
using Model.Core.Interfaces;
using Model.Data; 

namespace Model.Core
{
    public partial class GameEngine
    {
        public Player Player { get; private set; } = null!;
        public List<Platform> Platforms { get; private set; } = new List<Platform>();
        public double Score { get; private set; }
        public bool IsGameOver { get; private set; }
        public int Difficulty { get; set; } = 1;
        public event Action? OnGameOver;

        public const double CanvasWidth = 500;
        public const double CanvasHeight = 850;

        public GameEngine() { InitGame(); }

        private void InitGame()
        {
            Player = new Player(CanvasWidth / 2 - 20, CanvasHeight - 150);
            Score = 0;
            IsGameOver = false;
            GenerateInitialPlatforms();
        }

        private void GenerateInitialPlatforms()
        {
            Platforms.Clear();
            Random rnd = new Random();
            
            // 1. Платформа старта (видимая)
            double currentY = CanvasHeight - 120; 
            Platforms.Add(new NormalPlatform(CanvasWidth / 2 - 75, currentY, 3)); 

            Player.X = CanvasWidth / 2 - 17;
            Player.Y = currentY - 80;
            Player.VelocityY = 0;

            
            
        }

        public void Update(double deltaTime)
        {
            if (IsGameOver) return;
            Player.Update(deltaTime);

            if (Player.X + Player.Width < 0) Player.X = CanvasWidth;
            if (Player.X > CanvasWidth) Player.X = -Player.Width;

            CheckCollisions();
            HandleScrolling();

            if (Player.Y > CanvasHeight)
            {
                IsGameOver = true;
                OnGameOver?.Invoke(); 
            }
        }

        private void CheckCollisions()
        {
            if (Player.VelocityY > 0) 
            {
                foreach (var platform in Platforms)
                {
                    if (!platform.IsActive) continue;

                    if (Player.X + Player.Width > platform.X && 
                        Player.X < platform.X + platform.Width &&
                        Player.Y + Player.Height >= platform.Y && 
                        Player.Y + Player.Height <= platform.Y + 30)
                    {
                        Player.Y = platform.Y - Player.Height;
                        platform.OnCollision(Player);
                        break;
                    }
                }
            }
        }

        private void HandleScrolling()
        {
            double limit = CanvasHeight / 2;
            if (Player.Y < limit)
            {
                double offset = limit - Player.Y;
                Player.Y = limit; 
                foreach (var platform in Platforms) platform.Y += offset;
                Score += offset; 
                ManagePlatforms();
            }
        }

        private void ManagePlatforms()
        {
            Random rnd = new Random();
            Platforms.RemoveAll(p => p.Y > CanvasHeight + 100);

            // Только 1 платформа за раз при необходимости
            if (Platforms.Count > 0 && Platforms[0].Y > -200)
            {
                double nextY = Platforms[0].Y - rnd.Next(180, 230);
                double x = rnd.Next(50, (int)CanvasWidth - 160);
                
                int chance = rnd.Next(100);
                if (chance < (Difficulty * 4)) Platforms.Insert(0, new TrapPlatform(x, nextY));
                else if (chance < 15) Platforms.Insert(0, new SpringPlatform(x, nextY));
                else Platforms.Insert(0, new NormalPlatform(x, nextY, rnd.Next(1, 4)));
            }
        }

        public void LoadState(GameState state)
        {
            this.Score = state.Score;
            this.Difficulty = state.Difficulty;
            this.Player.X = state.PlayerX;
            this.Player.Y = state.PlayerY;
            this.Player.VelocityY = 0; 
            this.Platforms.Clear();
            foreach (var pData in state.Platforms)
            {
                Platform platform;
                switch (pData.Type) {
                    case "SpringPlatform": platform = new SpringPlatform(pData.X, pData.Y); break;
                    case "TrapPlatform": platform = new TrapPlatform(pData.X, pData.Y); break;
                    default: platform = new NormalPlatform(pData.X, pData.Y, 2); break;
                }
                this.Platforms.Add(platform);
            }
        }
    }
}