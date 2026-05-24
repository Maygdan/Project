using Model.Data;
namespace Model.Core
{
    public partial class GameEngine
    {
        public Player Player { get; private set; } = null!; // Сам рыцарь
        public List<Platform> Platforms { get; private set; } = new List<Platform>(); // Все площадки на экране
        public double Score { get; private set; } // Текущие очки
        public bool IsGameOver { get; private set; } // Флаг: проиграли или нет
        public int Difficulty { get; set; } = 1; // Уровень сложности
        public event Action? OnGameOver; // Сигнал о смерти для экрана

        public const double CanvasWidth = 500; // Ширина мира
        public const double CanvasHeight = 850; // Высота мира

        public GameEngine() { InitGame(); } // Создание движка

        private void InitGame() // Начальная настройка игры
        {
            Player = new Player(CanvasWidth / 2 - 20, CanvasHeight - 150);
            Score = 0;
            IsGameOver = false;
            GenerateInitialPlatforms(); // Построение первых камней
        }

        private void GenerateInitialPlatforms() // Создание стартового набора платформ
        {
            Platforms.Clear();
            Random rnd = new Random();
            
            double currentY = CanvasHeight - 120; // Высота первой платформы
            Platforms.Add(new NormalPlatform(CanvasWidth / 2 - 75, currentY, 3)); // Стартовая площадка

            Player.X = CanvasWidth / 2 - 17;
            Player.Y = currentY - 80; // Ставим рыцаря сверху
            Player.VelocityY = 0;

           
        }

        public void Update(double deltaTime) // Обновление всей математики игры
        {
            if (IsGameOver) return;
            Player.Update(deltaTime); // Считаем падение/бег игрока

            // Переход через края экрана
            if (Player.X + Player.Width < 0) Player.X = CanvasWidth;
            if (Player.X > CanvasWidth) Player.X = -Player.Width;

            CheckCollisions(); // Проверка: наступил ли на камень
            HandleScrolling(); // Сдвиг мира вниз

            if (Player.Y > CanvasHeight) // Если упал в бездну
            {
                IsGameOver = true;
                OnGameOver?.Invoke(); // Отправляем сигнал о смерти
            }
        }

        private void CheckCollisions() // Проверка касания платформ ногами
        {
            if (Player.VelocityY > 0) // Проверяем только когда летим вниз
            {
                foreach (var platform in Platforms)
                {
                    if (!platform.IsActive) continue; // Игнорируем сломанные

                    // Если ноги попали в зону платформы
                    if (Player.X + Player.Width > platform.X && 
                        Player.X < platform.X + platform.Width &&
                        Player.Y + Player.Height >= platform.Y && 
                        Player.Y + Player.Height <= platform.Y + 30)
                    {
                        Player.Y = platform.Y - Player.Height; // Ставим ровно на поверхность
                        platform.OnCollision(Player); // Запускаем прыжок
                        break;
                    }
                }
            }
        }

        private void HandleScrolling() // Движение мира за игроком вверх
        {
            double limit = CanvasHeight / 2;
            if (Player.Y < limit)
            {
                double offset = limit - Player.Y;
                Player.Y = limit; // Фиксируем рыцаря в центре
                foreach (var platform in Platforms) platform.Y += offset; // Двигаем камни вниз
                Score += offset; // Прибавляем очки за высоту
                ManagePlatforms(); // Чистим и добавляем новые камни
            }
        }

        private void ManagePlatforms() // Удаление старых и создание новых платформ
        {
            Random rnd = new Random();
            Platforms.RemoveAll(p => p.Y > CanvasHeight + 100); // Удаляем те, что улетели вниз

            if (Platforms.Count > 0 && Platforms[0].Y > -200) // Если сверху пусто
            {
                double nextY = Platforms[0].Y - rnd.Next(180, 230); // Новый шаг
                double x = rnd.Next(50, (int)CanvasWidth - 160);
                
                int chance = rnd.Next(100);
                // Выбор типа: ловушка, гриб или обычный камень
                if (chance < (Difficulty * 4)) Platforms.Insert(0, new TrapPlatform(x, nextY));
                else if (chance < 15) Platforms.Insert(0, new SpringPlatform(x, nextY));
                else Platforms.Insert(0, new NormalPlatform(x, nextY, rnd.Next(1, 4)));
            }
        }

        public void LoadState(GameState state) // Восстановление игры из файла
        {
            this.Score = state.Score;
            this.Difficulty = state.Difficulty;
            this.Player.X = state.PlayerX;
            this.Player.Y = state.PlayerY;
            this.Player.VelocityY = 0; 
            this.Platforms.Clear();
            foreach (var pData in state.Platforms) // Расстановка загруженных камней
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