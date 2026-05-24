using Model.Core.Interfaces;

namespace Model.Core
{
    public abstract class Platform : GameObject, IPlatform
    {
        // Флаг: активна ли площадка (можно ли от неё прыгнуть)
        public bool IsActive { get; protected set; } = true;

        protected Platform(double x, double y, double width, double height) 
            : base(x, y, width, height) { } // Общий конструктор для всех видов

        // Обязательный метод для всех видов камней: что делать при касании
        public abstract void OnCollision(Player player);
    }
}