using Newtonsoft.Json;
using System.IO;

namespace Model.Data
{
    public class JsonGameSerializer<T> : BaseSerializer<T>
    {
        public override void Serialize(string filePath, T data)
        {
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        public override T Deserialize(string filePath)
        {
            EnsureFileExists(filePath);
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}