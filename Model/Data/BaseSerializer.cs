using System.IO;

namespace Model.Data
{
    // Обобщенный абстрактный класс (Требование на "5" баллов)
    public abstract class BaseSerializer<T>
    {
        public abstract void Serialize(string filePath, T data);
        public abstract T Deserialize(string filePath);

        // Вспомогательный метод: проверка файла
        protected void EnsureFileExists(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл сохранения не найден");
        }
    }
}