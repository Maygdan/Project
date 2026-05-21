using Model.Core.Interfaces;

namespace Model.Core
{
    namespace Model.Core
{
    public class Player : GameObject
    {
        public double VelocityY { get; set; }
        public double VelocityX { get; set; }
        
        private const double Gravity = 1200; 
        private const double JumpForce = -700; // Усиленный прыжок для больших дистанций

        // Хитбокс 35x15 — это область в районе ног рыцаря
        public Player(double x, double y) : base(x, y, 35, 15) { }

        public override void Update(double deltaTime)
        {
            VelocityY += Gravity * deltaTime;
            Y += VelocityY * deltaTime;
            X += VelocityX * deltaTime;
        }

        public void Jump() => VelocityY = JumpForce;
        public void Jump(double force) => VelocityY = -Math.Abs(force);
    }
}