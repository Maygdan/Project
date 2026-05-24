namespace Model.Core
{
    // Структура для работы с точками в пространстве
    public struct Vector2D
    {
        public double X { get; set; } // Координата влево-вправо
        public double Y { get; set; } // Координата вверх-вниз

        public Vector2D(double x, double y) { X = x; Y = y; } // Создание вектора

        // Математическое сложение двух точек (Перегрузка оператора +)
        public static Vector2D operator +(Vector2D a, Vector2D b)
            => new Vector2D(a.X + b.X, a.Y + b.Y);

        // Математическое вычитание двух точек (Перегрузка оператора -)
        public static Vector2D operator -(Vector2D a, Vector2D b)
            => new Vector2D(a.X - b.X, a.Y - b.Y);
    }
}