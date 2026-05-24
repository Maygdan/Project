namespace Model.Core
{
    // Вторая часть главного движка (разделение обязанностей)
    public partial class GameEngine
    {
        // Создание текста для окна проигрыша
        public string GetGameOverMessage()
        {
            return $"Игра окончена!\nВаш счёт: {(int)Score}\nХотите попробовать снова?";
        }
    }
}