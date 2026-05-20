using Model.Core.Interfaces;

namespace Model.Core
{
    // Абстрактный класс №2
    public abstract class Platform : GameObject, IPlatform
    {
        // Свойство, чтобы знать, можно ли еще взаимодействовать с платформой
        public bool IsActive { get; protected set; } = true;

        protected Platform(double x, double y, double width, double height) 
            : base(x, y, width, height) { }

        // Метод, который определяет, что происходит при столкновении с игроком
        public abstract void OnCollision(Player player);
    }
}