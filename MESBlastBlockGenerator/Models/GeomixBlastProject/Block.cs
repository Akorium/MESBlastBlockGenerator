using Avalonia.Platform.Storage;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Block
    {
        [XmlAttribute("BlockID")]
        public string BlockId { get; set; } = string.Empty;
        [XmlElement("Points")]
        public Points Points { get; set; } = new();
        [XmlElement("Wels")]
        public Wells Wells { get; set; } = new();
    }
}