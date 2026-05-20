using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.Data
{
    // Упрощенный класс записи: только то, что просил
    public class ScoreEntry
    {
        public int Score { get; set; }
        public int Difficulty { get; set; }
    }

    public class ScoreManager
    {
        private const string FileName = "highscores.json";
        private List<ScoreEntry> _scores = new();
        private JsonGameSerializer<List<ScoreEntry>> _serializer = new();

        // Теперь принимаем и счёт, и сложность
        public void AddScore(int score, int difficulty)
        {
            _scores = Load(); // Загружаем старые данные, чтобы не перезаписывать их
            
            _scores.Add(new ScoreEntry { Score = score, Difficulty = difficulty });
            
            // Сортируем: сначала самые большие очки, берем только ТОП-10
            _scores = _scores.OrderByDescending(s => s.Score).Take(10).ToList();
            
            Save();
        }

        public void Save() => _serializer.Serialize(FileName, _scores);
        
        public List<ScoreEntry> Load() 
        {
            try 
            { 
                if (!System.IO.File.Exists(FileName)) return new List<ScoreEntry>();
                return _serializer.Deserialize(FileName); 
            }
            catch { return new List<ScoreEntry>(); }
        }
    }
}