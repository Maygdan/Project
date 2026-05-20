using System.IO;
using System.Xml.Serialization;

namespace Model.Data
{
    public class XmlGameSerializer<T> : BaseSerializer<T>
    {
        private XmlSerializer _serializer = new XmlSerializer(typeof(T));

        public override void Serialize(string filePath, T data)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                _serializer.Serialize(fs, data);
            }
        }

        public override T Deserialize(string filePath)
        {
            EnsureFileExists(filePath);
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                return (T)_serializer.Deserialize(fs)!;
            }
        }
    }
}