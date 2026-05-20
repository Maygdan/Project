namespace Model.Core
{
    // Вторая часть partial класса (требование ТЗ)
    public partial class GameEngine
    {
        public string GetGameOverMessage()
        {
            return $"Игра окончена!\nВаш счёт: {(int)Score}\nХотите попробовать снова?";
        }

        // Здесь в будущем можно добавить проверку рекордов
    }
}