namespace Model.Core
{
    public class TrapPlatform : Platform
    {
        public TrapPlatform(double x, double y) : base(x, y, 100, 5) { }

        public override void Update(double deltaTime) { }

        public override void OnCollision(Player player)
        {
            if (player.VelocityY > 0)
            {
                player.Jump(); 
                // Только здесь платформа становится "фантомной"
                this.IsActive = false; 
            }
        }
    }
}