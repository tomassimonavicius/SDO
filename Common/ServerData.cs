namespace Common
{
    [System.Xml.Serialization.XmlRoot("serverdata")]
    public class ServerData
    {
        [System.Xml.Serialization.XmlElement("id")]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElement("text")]
        public string Text { get; set; }
    }
}
