namespace Model.Data
{
    public class AppConfig
    {
        public string SavePath { get; set; } = "saves/";
        public bool UseXml { get; set; } = false;
        public bool IsFirstRun { get; set; } = true;
    }
}