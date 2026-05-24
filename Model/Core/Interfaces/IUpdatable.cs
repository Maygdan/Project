namespace Model.Core.Interfaces
{
    // Правило для предметов, которые должны постоянно обновляться
    public interface IUpdatable
    {
        // Метод для пересчета положения или логики объекта
        void Update(double deltaTime);
    }
}