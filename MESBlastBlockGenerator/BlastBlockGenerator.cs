using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using NLog;
using MESBlastBlockGenerator.Models.SOAP;
using MESBlastBlockGenerator.Models.BlastProject;

namespace MESBlastBlockGenerator
{
    public class BlastBlockGenerator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static string GenerateXmlContent(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance,
                                       string pitName, int level, int blockNumber)
        {
            var mesPmv = new MesPmv
            {
                HolesInBlastProject = new HolesInBlastProject
                {
                    Holes = GenerateHoles(maxRow, maxCol, rotationAngleDegrees, baseX, baseY, distance, pitName, level, blockNumber)
                }
            };
            logger.Debug("Генерация скважин завершена");
            var innerXml = SerializeMessage(mesPmv);
            logger.Debug("Сообщение mes_pmw сериализовано");
            var envelope = new Envelope
            {
                Body = new Body
                {
                    SoapXmlRequest = new SoapXmlRequest
                    {
                        XmlRequest = new XmlRequest
                        {
                            Message = new Message { XmlContent = innerXml }
                        }
                    }
                }
            };
            var soapXml = SerializeToSoap(envelope);
            return soapXml;
        }
        private static string SerializeMessage(MesPmv mesPmv)
        {
            logger.Debug("Инициализирована сериализация сообщения mes_pmv");
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, settings);
            var serializer = new XmlSerializer(typeof(MesPmv));
            serializer.Serialize(writer, mesPmv, ns);
            return stream.ToString();
        }
        private static string SerializeToSoap(Envelope envelope)
        {
            logger.Debug("Инициализирована сериализация SOAP-конверта");
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                Indent = true,
                OmitXmlDeclaration = true
            };

            var ns = new XmlSerializerNamespaces();
            ns.Add("x", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("tem", "http://tempuri.org/");

            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, settings);
            var serializer = new XmlSerializer(typeof(Envelope));
            serializer.Serialize(writer, envelope, ns);
            return stream.ToString();
        }
        private static List<Hole> GenerateHoles(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance, string pitName, int level, int blockNumber)
        {
            logger.Debug("Инициализирована генерация скважин");
            string blastProjectId = Guid.NewGuid().ToString();
            double rotationAngleRad = rotationAngleDegrees * Math.PI / 180.0;
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            var holes = new List<Hole>();
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    double x = baseX + (col) * distance;
                    double y = baseY + (row) * distance;

                    double relX = x - baseX;
                    double relY = y - baseY;

                    x = baseX + relX * cosAngle - relY * sinAngle;
                    y = baseY + relX * sinAngle + relY * cosAngle;

                    var hole = new Hole
                    {
                        HoleItem = new HoleItem
                        {
                            BlastProjectId = blastProjectId,
                            HoleId = Guid.NewGuid().ToString(),
                            HoleNumber = $"{row:D2}{col:D2}",
                            PitCode = pitName,
                            PitName = pitName,
                            LevelCode = $"{pitName}{level}",
                            LevelName = $"{level}",
                            BlockCode = $"{pitName}{level}-{blockNumber}",
                            BlockName = $"{level}-{blockNumber}",
                            BlockDrillingCode = $"{pitName}{pitName}{level}{level}-{blockNumber}Drill",
                            BlockDrillingName = $"{level}-{blockNumber}",
                            BlockBlastingCode = $"{pitName}{level}-{blockNumber}Blast",
                            BlockBlastingName = $"{level}-{blockNumber}",
                            X = x.ToString(CultureInfo.InvariantCulture),
                            Y = y.ToString(CultureInfo.InvariantCulture),
                            XFact = x.ToString(CultureInfo.InvariantCulture),
                            YFact = y.ToString(CultureInfo.InvariantCulture)
                        }
                    };
                    holes.Add(hole);
                }
            }
            return holes;
        }
    }
}