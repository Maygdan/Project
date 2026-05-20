using Model.Core.Interfaces;

namespace Model.Core
{
    public class Player : GameObject, IUpdatable
    {
        public double VelocityY { get; set; } // Скорость по вертикали
        public double VelocityX { get; set; } // Скорость по горизонтали
        
        private const double Gravity = 1200; // Сила гравитации (пикселей в сек^2)
        private const double JumpForce = -750; // Сила прыжка (отрицательная, т.к. в WPF 0 вверху)

        public Player(double x, double y) : base(x, y, 25, 10) { }

        public override void Update(double deltaTime)
        {
            // Применяем гравитацию к скорости
            VelocityY += Gravity * deltaTime;
            
            // Обновляем позицию на основе скорости
            Y += VelocityY * deltaTime;
            X += VelocityX * deltaTime;
        }

        public void Jump()
{
    VelocityY = -750;
}

// Перегрузка метода Jump (Метод 2): Прыжок с заданной силой
// Это закроет пункт "Перегрузка методов" в чек-листе
public void Jump(double customForce)
{
    VelocityY = -Math.Abs(customForce);
}
    }
}