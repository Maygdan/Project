namespace Model.Core
{
    // Абстрактный класс №1
    public abstract class GameObject
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        protected GameObject(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // Абстрактный метод, который каждый объект реализует по-своему
        // Например, игрок падает, а платформа может двигаться или ломаться
        public abstract void Update(double deltaTime);
    }
}