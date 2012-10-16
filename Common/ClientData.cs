using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [System.Xml.Serialization.XmlRoot("data")]
    public class ClientData
    {
        [System.Xml.Serialization.XmlElement("id")]
        public string Id { get; set; }
        [System.Xml.Serialization.XmlElement("text")]
        public string Text { get; set; }
        [System.Xml.Serialization.XmlElement("key")]
        public string Key { get; set; }
        [System.Xml.Serialization.XmlElement("iv")]
        public string IV { get; set; }
        [System.Xml.Serialization.XmlElement("algorithm")]
        public string Algorithm { get; set; }
        [System.Xml.Serialization.XmlElement("action")]
        public string Action { get; set; }
    }
}
