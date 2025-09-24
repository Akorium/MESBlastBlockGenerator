using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    [XmlRoot("Projects")]
    internal class GeomixBlastProjects
    {
        [XmlElement("Project")]
        public List<Project> ProjectList { get; set; } = [];
    }
}
