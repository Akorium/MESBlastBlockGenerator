using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    public class Blocks
    {
        [XmlElement("Block")]
        public List<Block> BlockList { get; set; } = [];
    }
}