using System.Xml.Serialization;
using System.Xml;

namespace MESBlastBlockGenerator.Models.SOAP
{
    public class Message
    {
        [XmlIgnore]
        public string XmlContent { get; set; }

        [XmlText]
        public XmlNode[] CDataContent
        {
            get
            {
                if (string.IsNullOrEmpty(XmlContent))
                    return null;

                var doc = new XmlDocument();
                // Добавляем XML-декларацию в CDATA
                string contentWithDeclaration = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + XmlContent;
                return [doc.CreateCDataSection(contentWithDeclaration)];
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    XmlContent = null;
                    return;
                }

                // Удаляем XML-декларацию при десериализации, если она есть
                string content = value[0].Value ?? string.Empty;
                if (content.StartsWith("<?xml"))
                {
                    int end = content.IndexOf("?>") + 2;
                    XmlContent = content[end..].TrimStart();
                }
                else
                {
                    XmlContent = content;
                }
            }
        }
    }
}