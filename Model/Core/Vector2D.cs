namespace Model.Core
{
    // Обычная структура для координат (чтобы работала перегрузка)
    public struct Vector2D
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector2D(double x, double y) { X = x; Y = y; }

        // Теперь это скомпилируется (Перегрузка оператора №1)
        public static Vector2D operator +(Vector2D a, Vector2D b)
            => new Vector2D(a.X + b.X, a.Y + b.Y);

        public static Vector2D operator -(Vector2D a, Vector2D b)
            => new Vector2D(a.X - b.X, a.Y - b.Y);
    }
}