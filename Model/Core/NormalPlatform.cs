namespace Model.Core
{
    public class NormalPlatform : Platform
    {
        public NormalPlatform(double x, double y, int type = 1) : base(x, y, 60, 15) 
{ 
    
    if (type == 1) Width = 80;   // Маленькая
    if (type == 2) Width = 110;  // Средняя
    if (type == 3) Width = 150;  // Большая
}

        public override void Update(double deltaTime) { }

        public override void OnCollision(Player player) 
        { 
            if (player.VelocityY > 0) player.Jump(); 
        }
    }
}