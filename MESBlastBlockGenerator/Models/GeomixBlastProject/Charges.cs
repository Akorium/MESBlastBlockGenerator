using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Charges
    {
        [XmlElement("Charge")]
        public List<Charge> ChargeList { get; set; } = [];
    }
}