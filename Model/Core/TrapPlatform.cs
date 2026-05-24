namespace Model.Core
{
    // Площадка-ловушка (ломается после прыжка)
    public class TrapPlatform : Platform
    {
        public TrapPlatform(double x, double y) : base(x, y, 100, 5) { } // Создание хрупкого камня

        public override void Update(double deltaTime) { } // Камень статичен

        // Логика поломки при касании
        public override void OnCollision(Player player)
        {
            if (player.VelocityY > 0)
            {
                player.Jump(); // Обычный прыжок
                
                // Камень становится неактивным и исчезает с экрана
                this.IsActive = false; 
            }
        }
    }
}