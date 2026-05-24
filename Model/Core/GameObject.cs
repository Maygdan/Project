using Model.Data;
namespace Model.Core
{
    // Главный шаблон для любого предмета в игре
    public abstract class GameObject
    {
        public double X { get; set; } // Позиция по горизонтали
        public double Y { get; set; } // Позиция по вертикали
        public double Width { get; set; } // Ширина объекта
        public double Height { get; set; } // Высота объекта

        // Создание объекта с заданными размерами и координатами
        protected GameObject(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        // Обязательный метод обновления, который каждый предмет делает по-своему
        public abstract void Update(double deltaTime);
    }
}