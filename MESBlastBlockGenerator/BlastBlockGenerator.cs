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
    /// <summary>
    /// Класс для генерации XML в формате MES
    /// </summary>
    public class BlastBlockGenerator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly XmlWriterSettings xmlSettings = new()
        {
            Encoding = Encoding.UTF8,
            Indent = true,
            OmitXmlDeclaration = true
        };

        /// <summary>
        /// Точка входа для генерации XML в формате MES
        /// </summary>
        /// <param name="maxRow">Количество рядов скважин</param>
        /// <param name="maxCol">Количество столбцов скважин</param>
        /// <param name="rotationAngleDegrees">Угол поворота сетки скважин</param>
        /// <param name="baseX">X первой скважины блока</param>
        /// <param name="baseY">Y первой скважины блока</param>
        /// <param name="distance">Расстояние между скважинами</param>
        /// <param name="pitName">Название карьера</param>
        /// <param name="level">Уровень проекта</param>
        /// <param name="blockNumber">Порядковый номер блока</param>
        /// <returns>XML в формате string</returns>
        public static string GenerateXmlContent(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance,
                                       string pitName, int level, int blockNumber, bool dispersedCharge)
        {
            //Форматирование самого сообщения и SOAP-конверта отличаются, поэтому мы их сериализуем отдельно
            var mesPmv = new MesPmv
            {
                HolesInBlastProject = new HolesInBlastProject
                {
                    Holes = GenerateHoles(maxRow, maxCol, rotationAngleDegrees, baseX, baseY, distance, pitName, level, blockNumber, dispersedCharge)
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
        /// <summary>
        /// Сериализует сообщение mes_pmv
        /// </summary>
        /// <param name="mesPmv">Объект MesPmv</param>
        /// <returns>Сериализованный объект MesPmv в формате string</returns>
        private static string SerializeMessage(MesPmv mesPmv)
        {
            logger.Debug("Инициализирована сериализация сообщения mes_pmv");
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, xmlSettings);
            var serializer = new XmlSerializer(typeof(MesPmv));
            serializer.Serialize(writer, mesPmv, ns);
            return stream.ToString();
        }
        /// <summary>
        /// Сериализует итоговый XML с учётом SOAP-конверта
        /// </summary>
        /// <param name="envelope">Объект Envelope(SOAP-конверт)</param>
        /// <returns>Сериализованный XML в формате string</returns>
        private static string SerializeToSoap(Envelope envelope)
        {
            logger.Debug("Инициализирована сериализация SOAP-конверта");
            var ns = new XmlSerializerNamespaces();
            ns.Add("x", "http://schemas.xmlsoap.org/soap/envelope/");
            ns.Add("tem", "http://tempuri.org/");

            using var stream = new StringWriter();
            using var writer = XmlWriter.Create(stream, xmlSettings);
            var serializer = new XmlSerializer(typeof(Envelope));
            serializer.Serialize(writer, envelope, ns);
            return stream.ToString();
        }

        /// <summary>
        /// Генерирует список скважин для блока
        /// </summary>
        /// <param name="maxRow">Количество рядов скважин</param>
        /// <param name="maxCol">Количество столбцов скважин</param>
        /// <param name="rotationAngleDegrees">Угол поворота сетки скважин</param>
        /// <param name="baseX">X первой скважины блока</param>
        /// <param name="baseY">Y первой скважины блока</param>
        /// <param name="distance">Расстояние между скважинами</param>
        /// <param name="pitName">Название карьера</param>
        /// <param name="level">Уровень проекта</param>
        /// <param name="blockNumber">Порядковый номер блока</param>
        /// <returns>Список объектов Hole</returns>
        private static List<Hole> GenerateHoles(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance, string pitName, int level, int blockNumber, bool dispersedCharge)
        {
            logger.Debug("Инициализирована генерация скважин");
            string blastProjectId = Guid.NewGuid().ToString();
            double rotationAngleRad = rotationAngleDegrees * Math.PI / 180.0;
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            var holes = new List<Hole>(maxRow * maxCol);
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    (double x, double y) = CalculateHoleCoords(baseX, baseY, distance, cosAngle, sinAngle, row, col);

                    var hole = GenerateHole(pitName, level, blockNumber, blastProjectId, row, col, x, y, dispersedCharge);
                    holes.Add(hole);
                }
            }
            return holes;
        }

        /// <summary>
        /// Генерирует скважин для блока
        /// </summary>
        /// <param name="pitName">Название карьера</param>
        /// <param name="level">Уровень проекта</param>
        /// <param name="blockNumber">Порядковый номер блока</param>
        /// <param name="blastProjectId">GUID проекта</param>
        /// <param name="row">Ряд скважины</param>
        /// <param name="col">Столбец скважины</param>
        /// <param name="x">X координата скважины</param>
        /// <param name="y">Y координата скважины</param>
        /// <returns>Объект Hole</returns>
        private static Hole GenerateHole(string pitName, int level, int blockNumber, string blastProjectId, int row, int col, double x, double y, bool dispersedCharge)
        {
            HoleItem holeItem = new()
            {
                //MES передаёт все поля в string
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
            };
            if (dispersedCharge)
            {
                List<Material> materials =
                [
                    new(),
                    new Material
                    {
                        MaterialCode = "1025160",
                        MaterialShortName = "Вещество взрывчатое Березит Э-70",
                        IsExplosive = "true",
                        MaterialDensity = "1200",
                        CupDensity = "0",
                        Amounts =
                        [
                            new Amount
                            {
                                Value = "500",
                                Priority = "1"
                            },
                            new Amount
                            {
                                Value = "600",
                                Priority = "2"
                            }
                        ]
                    }
                ];
                return new Hole
                {
                    HoleItem = holeItem,
                    PlanChargeMaterials = materials,
                };
            }
            else
            {
                return new Hole
                {
                    HoleItem = holeItem,
                };
            }

            
        }

        /// <summary>
        /// Расчитывает координаты скважины
        /// </summary>
        /// <param name="baseX">X первой скважины блока</param>
        /// <param name="baseY">Y первой скважины блока</param>
        /// <param name="distance">Расстояние между скважинами</param>
        /// <param name="cosAngle">Косинус угла поворота блока</param>
        /// <param name="sinAngle">Синус угла поворота блока</param>
        /// <param name="row">Ряд скважины</param>
        /// <param name="col">Столбец скважины</param>
        /// <returns>Координаты X и Y</returns>
        private static (double x, double y) CalculateHoleCoords(double baseX, double baseY, double distance, double cosAngle, double sinAngle, int row, int col)
        {
            double x = baseX + (col) * distance;
            double y = baseY + (row) * distance;

            double relX = x - baseX;
            double relY = y - baseY;

            x = baseX + relX * cosAngle - relY * sinAngle;
            y = baseY + relX * sinAngle + relY * cosAngle;

            return (x, y);
        }
    }
}