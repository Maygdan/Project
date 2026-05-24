namespace Model.Core
{
    // Площадка-пружина (подбрасывает высоко)
    public class SpringPlatform : Platform
    {
        public SpringPlatform(double x, double y) : base(x, y, 60, 5) { } // Создание гриба

        public override void Update(double deltaTime) { } // Гриб не двигается сам

        // Логика мощного прыжка при касании
        public override void OnCollision(Player player)
        {
            if (player.VelocityY > 0) 
            {
                // Запуск рыцаря вверх с удвоенной силой
                player.Jump(1200); 
            }
        }
    }
}