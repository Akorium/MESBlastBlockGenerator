using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Points
    {
        [XmlElement("Point")]
        public List<Point> Point { get; set; } = [];
    }
}