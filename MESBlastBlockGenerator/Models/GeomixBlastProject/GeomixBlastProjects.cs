using System.Collections.Generic;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Models.GeomixBlastProject
{
    [XmlRoot("Projects")]
    public class GeomixBlastProjects
    {
        [XmlElement("Project")]
        public List<Project> ProjectList { get; set; } = [];
    }
}
