namespace Model.Core
{
    public class Player : GameObject
    {
        public double VelocityY { get; set; }
        public double VelocityX { get; set; }
        
        private const double Gravity = 1150; 
        private const double JumpForce = -850; // Сила, чтобы точно долетать

        public Player(double x, double y) : base(x, y, 35, 5) { } // Хитбокс только в ногах

        public override void Update(double deltaTime)
        {
            VelocityY += Gravity * deltaTime;
            if (VelocityY > 1250) VelocityY = 1250; // Лимит скорости падения (защита от пролета сквозь пол)

            Y += VelocityY * deltaTime;
            X += VelocityX * deltaTime;
        }

        public void Jump() => VelocityY = JumpForce;
        public void Jump(double force) => VelocityY = -Math.Abs(force);
    }
}