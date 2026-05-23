using System;
using System.Collections.Generic;
using System.Linq;
using System.IO; // Для работы с папками

namespace Model.Data
{
    // Класс записи: только счёт и сложность
    public class ScoreEntry
    {
        public int Score { get; set; } // Очки игрока
        public int Difficulty { get; set; } // Уровень сложности
    }

    public class ScoreManager
    {
        private const string FolderName = "Game.info"; // Имя системной папки
        private const string FileName = "Game.info/highscores.json"; // Путь к файлу рекордов
        private List<ScoreEntry> _scores = new(); // Список всех рекордов
        private JsonGameSerializer<List<ScoreEntry>> _serializer = new(); // Инструмент записи

        // Добавление нового результата в таблицу
        public void AddScore(int score, int difficulty)
        {
            _scores = Load(); // Загружаем старые записи
            
            _scores.Add(new ScoreEntry { Score = score, Difficulty = difficulty }); // Добавляем новую
            
            // Сортируем: сначала самые большие очки, берем топ-10
            _scores = _scores.OrderByDescending(s => s.Score).Take(10).ToList();
            
            Save(); // Записываем на диск
        }

        // Сохранение таблицы в папку Game.info
        public void Save() 
        {
            if (!Directory.Exists(FolderName)) Directory.CreateDirectory(FolderName); // Создаем папку, если её нет
            _serializer.Serialize(FileName, _scores);
        }
        
        // Чтение таблицы из файла
        public List<ScoreEntry> Load() 
        {
            try 
            { 
                if (!File.Exists(FileName)) return new List<ScoreEntry>(); // Если файла нет, возвращаем пустой список
                return _serializer.Deserialize(FileName); 
            }
            catch { return new List<ScoreEntry>(); } // При ошибке возвращаем пустоту
        }
    }
}