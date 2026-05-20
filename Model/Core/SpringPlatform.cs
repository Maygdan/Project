namespace Model.Core
{
    public class SpringPlatform : Platform
    {
        public SpringPlatform(double x, double y) : base(x, y, 60, 5) { }

        public override void Update(double deltaTime) { }

        public override void OnCollision(Player player)
        {
            if (player.VelocityY > 0) 
            {
                // Используем перегруженный метод Jump с заданной силой
                player.Jump(1200); 
            }
        }
    }
}