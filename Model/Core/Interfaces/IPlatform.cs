namespace Model.Core.Interfaces
{
    // Общие правила для всех площадок
    public interface IPlatform
    {
        // Проверка: существует ли еще площадка или сломалась
        bool IsActive { get; }
    }
}