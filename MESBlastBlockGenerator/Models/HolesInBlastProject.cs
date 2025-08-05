using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public class HolesInBlastProject
    {
        [XmlElement(ElementName = "hole")]
        public required List<Hole> Holes { get; set; }
    }
}