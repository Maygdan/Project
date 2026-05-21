using System;
using System.Collections.Generic;

namespace Model.Data
{
    [Serializable] 
    public class PlatformSaveData
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Type { get; set; } = "";
    }

    [Serializable]
    public class GameState
    {
        public double PlayerX { get; set; }
        public double PlayerY { get; set; }
        public double Score { get; set; }
        public int Difficulty { get; set; }
        public List<PlatformSaveData> Platforms { get; set; } = new();
    }
}