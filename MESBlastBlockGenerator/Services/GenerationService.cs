using CsvHelper;
using CsvHelper.Configuration;
using MESBlastBlockGenerator.Enums;
using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Models.BlastProject;
using MESBlastBlockGenerator.Models.CSVBlastProject;
using MESBlastBlockGenerator.Models.GeomixBlastProject;
using MESBlastBlockGenerator.Models.MESBlastProject;
using MESBlastBlockGenerator.Models.SOAP;
using MESBlastBlockGenerator.Models.SOAP.Request;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator.Services
{
    /// <summary>
    /// This service is responsible for generating XML data for MES or Geomix mass explosion projects.
    /// </summary>
    public class GenerationService(IXmlSerializationService serializationService, ICoordinateCalculatorService coordinateCalculator) : IGenerationService
    {
        private readonly ICoordinateCalculatorService _coordinateCalculator = coordinateCalculator;
        private readonly IXmlSerializationService _serializationService = serializationService;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private static readonly XmlSerializerNamespaces _soapNamespaces = new(
            [
                new XmlQualifiedName("x", "http://schemas.xmlsoap.org/soap/envelope/"),
                new XmlQualifiedName("tem", "http://tempuri.org/")
            ]);
        private static readonly CsvConfiguration _csvConfiguration = new(CultureInfo.InvariantCulture) { Delimiter = ";" };
        /// <summary>
        /// Generates MES mass explosion project in XML format.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>The generated MES mass explosion project in XML format.</returns>
        public string GenerateMESMassExplosionProject(InputParameters inputs)
        {
            _logger.Debug("Generating MES mass explosion project message...");
            var mesMessage = GenerateMESMassExplosionProjectMessage(inputs);
            _logger.Debug("MES mass explosion project message generated. Serializing...");
            var innerXml = _serializationService.Serialize(mesMessage, new XmlSerializerNamespaces());
            _logger.Debug("MES mass explosion project message serialized. Creating SOAP envelope...");
            var envelope = CreateSoapEnvelope(innerXml);
            _logger.Debug("SOAP envelope created. Serializing...");

            return _serializationService.Serialize(envelope, _soapNamespaces);
        }
        /// <summary>
        /// Generates Geomix mass explosion project in XML format.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>The generated Geomix mass explosion project in XML format.</returns>
        public string GenerateGeomixMassExplosionProject(InputParameters inputs)
        {
            _logger.Debug("Generating Geomix mass explosion project...");
            var geomixMassExplosionProject = GenerateGeomixBlastProjects(inputs);
            _logger.Debug("Geomix mass explosion project generated. Serializing...");
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");
            return _serializationService.Serialize(geomixMassExplosionProject, namespaces);
        }

        /// <summary>
        /// Generates CSV data in the specified format for blast project using CsvHelper and GenerateGridObjects.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>CSV formatted string with blast project data.</returns>
        public (string blastHoles, string blastBlockPoints) GenerateBlastProjectCsv(InputParameters inputs)
        {
            _logger.Debug("Generating CSV mass explosion project...");
            var blastHoles = GenerateGridObjects(inputs, CreateBlastHoleRecord);
            _logger.Debug("СSV mass explosion project generated. Generating points for mass explosion project...");
            var (cosAngle, sinAngle) = _coordinateCalculator.PrecalculateRotation(inputs.RotationAngle);
            var blastBlockPoints = GenerateBlastBlockPoints(inputs, cosAngle, sinAngle);
            _logger.Debug("Points for mass explosion project generated. Serializing...");
            return (SerializeCSV(blastHoles, new BlastHoleRecordMap()), SerializeCSV(blastBlockPoints, new BlastBlockPointRecordMap()));
        }

        /// <summary>
        /// Generates a list of objects for a grid based on the given input parameters.
        /// </summary>
        /// <typeparam name="T">The type of the objects to generate.</typeparam>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <param name="objectFactory">A function that creates an object for a given row, column, inputs, and coordinates.</param>
        /// <returns>A list of objects generated for the grid.</returns>
        private List<T> GenerateGridObjects<T>(InputParameters inputs, Func<int, int, InputParameters, (double, double), T> objectFactory)
        {
            var (cosAngle, sinAngle) = _coordinateCalculator.PrecalculateRotation(inputs.RotationAngle);
            var objects = new List<T>(checked((int)(inputs.MaxCol * inputs.MaxRow)));

            for (int row = 0; row < inputs.MaxRow; row++)
            {
                for (int col = 0; col < inputs.MaxCol; col++)
                {
                    var coords = _coordinateCalculator.CalculateCoords(inputs, cosAngle, sinAngle, row, col);
                    var obj = objectFactory(row, col, inputs, coords);
                    objects.Add(obj);
                }
            }
            return objects;
        }
        #region CSV Blast Project Generation
        /// <summary>
        /// Creates a BlastHoleRecord for the specified row, column, and inputs.
        /// </summary>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <param name="inputs">The input parameters.</param>
        /// <param name="coords">The calculated coordinates.</param>
        /// <returns>A BlastHoleRecord object.</returns>
        private static BlastHoleRecord CreateBlastHoleRecord(int row, int col, InputParameters inputs, (double x, double y) coords)
        {
            return new BlastHoleRecord
            {
                Name = $"{row:D2}{col:D2}",
                X = coords.x,
                Y = coords.y,
                Z = inputs.BaseZ,
                BlastBlockName = $"{inputs.PitName}{inputs.Level}-{inputs.BlockNumber}",
                DesignChargeMass = inputs.MainChargeMass,
                DesignChargeHeight = inputs.DesignDepth - inputs.StemmingLength,
                Depth = inputs.DesignDepth,
                Diameter = inputs.DesignDiameter * 0.001,
                Tamping = inputs.StemmingLength
            };
        }
        private static string SerializeCSV<T>(List<T> records, ClassMap map)
        {
            var csv = new StringBuilder();
            using (var writer = new StringWriter(csv))
            using (var csvWriter = new CsvWriter(writer, _csvConfiguration))
            {
                csvWriter.Context.RegisterClassMap(map);
                csvWriter.WriteRecords(records);
            }
            return csv.ToString();
        }
        private List<BlastBlockPointRecord> GenerateBlastBlockPoints(InputParameters inputs, double cosAngle, double sinAngle)
        {
            var cornerIndices = new (int row, int col)[]
            {
                (0, 0),
                (0, inputs.MaxCol-1),
                (inputs.MaxRow-1, inputs.MaxCol-1),
                (inputs.MaxRow-1, 0)
            };

            var points = new List<BlastBlockPointRecord>(4);
            int sequence = 0;

            foreach (var (row, col) in cornerIndices)
            {
                var (x, y) = _coordinateCalculator.CalculateCoords(inputs, cosAngle, sinAngle, row, col);
                points.Add(new BlastBlockPointRecord
                {
                    BlastBlockName = $"{inputs.PitName}{inputs.Level}-{inputs.BlockNumber}",
                    Sequence = sequence++,
                    X = x,
                    Y = y
                });
            }
            return points;
        }
        #endregion

        #region MES Mass Explosion Project Generation
        /// <summary>
        /// Creates a SOAP envelope for MES mass explosion project.
        /// </summary>
        /// <param name="innerXml">MES mass explosion project message in XML format.</param>
        /// <returns>An Envelope object with the SOAP envelope structure.</returns>
        private static Envelope<RequestBody> CreateSoapEnvelope(string innerXml)
        {
            return new Envelope<RequestBody>
            {
                Header = new object(),
                Body = new RequestBody
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
        }
        /// <summary>
        /// Generates a MesPmv object with the given InputParameters.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A MesPmv object with a HolesInBlastProject object containing the generated holes.</returns>
        private MesPmv GenerateMESMassExplosionProjectMessage(InputParameters inputs)
        {
            _logger.Debug("Generating holes for MES mass explosion project message...");
            var holes = GenerateMesHoles(inputs);
            _logger.Debug("Holes for MES mass explosion project message generated.");
            return new MesPmv
            {
                HolesInBlastProject = new HolesInBlastProject { Holes = holes }
            };
        }
        /// <summary>
        /// This method generates a list of holes for a MES mass explosion project.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A list of holes for the project.</returns>
        private List<Hole> GenerateMesHoles(InputParameters inputs)
        {
            var blastProjectId = Guid.NewGuid().ToString();
            _logger.Debug($"Generating explosives for holes in MES mass explosion project...");
            var materials = GenerateMesMaterials(inputs);
            _logger.Debug($"Explosives for holes in MES mass explosion project generated.");
            return GenerateGridObjects(inputs, (row, col, inputParams, coords) =>
                GenerateMesHole(blastProjectId, row, col, inputParams, coords, materials));
        }
        /// <summary>
        /// Generates a Hole object for a MES mass explosion project.
        /// </summary>
        /// <param name="blastProjectId">The ID of the blast project.</param>
        /// <param name="row">The row number of the hole.</param>
        /// <param name="col">The column number of the hole.</param>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <param name="coords">The coordinates of the hole.</param>
        /// <param name="materials">The explosives used in the project.</param>
        /// <returns>A Hole object for the specified hole.</returns>
        private static Hole GenerateMesHole(string blastProjectId, int row, int col,
            InputParameters inputs, (double x, double y) coords,
            List<Models.MESBlastProject.Material> materials)
        {
            var (levelCode, blockName, blockCode) = GenerateBlockCodes(inputs);
            /*
            if (inputs.ChargeType == ChargeType.Рассредоточенный)
            {
                materials.Amounts.Add(new Amount
                {
                    Value = inputs.SecondaryChargeMass.ToString(CultureInfo.InvariantCulture),
                    Priority = "2"
                });
            }*/
            var hole = new Hole
            {
                HoleItem = new()
                {
                    BlastProjectId = blastProjectId,
                    HoleId = Guid.NewGuid().ToString(),
                    HoleNumber = $"{row:D2}{col:D2}",
                    PitCode = inputs.PitName,
                    PitName = inputs.PitName,
                    LevelCode = levelCode,
                    LevelName = inputs.Level.ToString(),
                    BlockCode = blockCode,
                    BlockName = blockName,
                    BlockDrillingCode = $"{inputs.PitName}{levelCode}{blockName}Drill",
                    BlockDrillingName = blockName,
                    BlockBlastingCode = $"{inputs.PitName}{blockName}Blast",
                    BlockBlastingName = blockName,
                    DepthPlan = inputs.DesignDepth.ToString(CultureInfo.InvariantCulture),
                    DiameterPlan = inputs.DesignDiameter.ToString(CultureInfo.InvariantCulture),
                    X = coords.x.ToString(CultureInfo.InvariantCulture),
                    Y = coords.y.ToString(CultureInfo.InvariantCulture),
                    Z = inputs.BaseZ.ToString(CultureInfo.InvariantCulture),
                    IsDrilling = inputs.IsDrilling.ToString().ToLower()
                },
                PlanChargeMaterials = materials,
                StemmingLengthPlan = new()
                {
                    Value = inputs.StemmingLength.ToString(CultureInfo.InvariantCulture)
                }
            };

            switch (inputs.HoleMaterialType)
            {
                case HoleMaterialType.Руда:
                    hole.HoleItem.HoleMaterial = "Скважины руды";
                    hole.HoleItem.HoleMaterialCode = "1028255";
                    break;
                case HoleMaterialType.Вскрыша:
                    hole.HoleItem.HoleMaterial = "Скважины вскрыши";
                    hole.HoleItem.HoleMaterialCode = "1028251";
                    break;
                default:
                    hole.HoleItem.HoleMaterial = "Взрывные скважины ВСДП";
                    hole.HoleItem.HoleMaterialCode = "1078066";
                    break;
            }

            if (inputs.IsDrilling)
            {
                hole.HoleItem.DepthFact = inputs.RealDepth.ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.DepthFactEomId = hole.HoleItem.DepthPlanEomId;
                hole.HoleItem.DepthFactEom = hole.HoleItem.DepthPlanEom;
                hole.HoleItem.DiameterFact = inputs.RealDiameter.ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.DiameterFactEomId = hole.HoleItem.DiameterEomId;
                hole.HoleItem.DiameterFactEom = hole.HoleItem.DiameterEom;
                hole.HoleItem.XFact = (coords.x + inputs.CoordinatesDeviation).ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.YFact = (coords.y - inputs.CoordinatesDeviation).ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.ZFact = (inputs.BaseZ + inputs.CoordinatesDeviation).ToString(CultureInfo.InvariantCulture);
            }
            return hole;
        }

        /// <summary>
        /// Generates a list of explosives for a MES mass explosion project.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A list of explosives for the project.</returns>
        private static List<Models.MESBlastProject.Material> GenerateMesMaterials(InputParameters inputs)
        {
            _logger.Debug($"Generating detonator for holes in MES mass explosion project...");
            var materials = new List<Models.MESBlastProject.Material>
            {
                new()
            };
            _logger.Debug($"Detonator for holes in MES mass explosion project generated. Generating main explosive...");
            var explosive = new Models.MESBlastProject.Material
            {
                MaterialCode = "1025160",
                MaterialShortName = "Вещество взрывчатое Березит Э-70",
                IsExplosive = "true",
                MaterialDensity = "1200",
                CupDensity = "0",
                Amounts =
                [
                    new() { Value = inputs.MainChargeMass.ToString(CultureInfo.InvariantCulture) }
                ]
            };

            if (inputs.ChargeType == ChargeType.Рассредоточенный)
            {
                _logger.Debug($"Main explosive for holes in MES mass explosion project generated. Generating secondary explosive...");
                explosive.Amounts.Add(new Amount
                {
                    Value = inputs.SecondaryChargeMass.ToString(CultureInfo.InvariantCulture),
                    Priority = "2"
                });
            }

            materials.Add(explosive);
            return materials;
        }

        /// <summary>
        /// Generates block codes based on the given input parameters.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A tuple containing the level code, block name, and block code.</returns>
        private static (string levelCode, string blockName, string blockCode) GenerateBlockCodes(InputParameters inputs)
        {
            var levelCode = $"{inputs.PitName}{inputs.Level}";
            var blockName = $"{inputs.Level}-{inputs.BlockNumber}";
            var blockCode = $"{levelCode}-{inputs.BlockNumber}";
            return (levelCode, blockName, blockCode);
        }
        #endregion

        #region Geomix Mass Explosion Project Generation
        /// <summary>
        /// This method generates a Geomix blast project based on the given input parameters.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A GeomixBlastProjects object representing the generated blast project.</returns>
        private GeomixBlastProjects GenerateGeomixBlastProjects(InputParameters inputs)
        {
            var projectId = $"{inputs.PitName}{inputs.Level}-{inputs.BlockNumber}";
            var (cosAngle, sinAngle) = _coordinateCalculator.PrecalculateRotation(inputs.RotationAngle);
            _logger.Debug($"Generating points for Geomix mass explosion project...");
            var blastProjectPoints = GenerateGeomixPoints(inputs, cosAngle, sinAngle);
            _logger.Debug($"Points for Geomix mass explosion project generated. Generating wells...");
            var wells = GenerateGeomixWells(inputs);
            _logger.Debug($"Wells for Geomix mass explosion project generated.");
            var project = new Project
            {
                ProjectId = projectId,
                Blocks = new Blocks
                {
                    BlockList =
                    [
                        new Block
                        {
                            BlockId = projectId,
                            Points = new Points { Point = blastProjectPoints },
                            Wells = new Wells { WellsList = wells }
                        }
                    ]
                }
            };

            return new GeomixBlastProjects { ProjectList = [project] };
        }
        /// <summary>
        /// Generates a list of wells for a Geomix mass explosion project.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A list of Well objects representing the wells in the project.</returns>
        private List<Well> GenerateGeomixWells(InputParameters inputs)
        {
            var (cosAngle, sinAngle) = _coordinateCalculator.PrecalculateRotation(inputs.RotationAngle);
            _logger.Debug($"Generating explosives for Geomix mass explosion project...");
            var charge = GenerateGeomixCharge(inputs);
            _logger.Debug($"Explosives for Geomix mass explosion project generated.");
            return GenerateGridObjects(inputs, (row, col, inputParams, coords) =>
                GenerateGeomixWell(row, col, inputParams, coords, charge));
        }

        /// <summary>
        /// Generates a Well object for a Geomix mass explosion project.
        /// </summary>
        /// <param name="row">The row number of the well.</param>
        /// <param name="col">The column number of the well.</param>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <param name="coords">The coordinates of the well.</param>
        /// <param name="charge">The list of charges for the well.</param>
        /// <returns>A Well object representing the well.</returns>
        private static Well GenerateGeomixWell(int row, int col, InputParameters inputs,
            (double x, double y) coords, List<Charge> charge)
        {

            return new Well
            {
                WellID = $"{row:D2}{col:D2}",
                WellNumber = $"{row:D2}{col:D2}",
                Depth = inputs.DesignDepth.ToString(CultureInfo.InvariantCulture),
                X = coords.x.ToString(CultureInfo.InvariantCulture),
                Y = coords.y.ToString(CultureInfo.InvariantCulture),
                Z = inputs.BaseZ.ToString(CultureInfo.InvariantCulture),
                DM = (inputs.DesignDiameter * 0.001).ToString(CultureInfo.InvariantCulture),
                Charges = new Charges { ChargeList = charge }
            };
        }

        /// <summary>
        /// Generates a list of charges for a Geomix mass explosion project.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <returns>A list of Charge objects representing the charges in the project.</returns>
        private static List<Charge> GenerateGeomixCharge(InputParameters inputs)
        {
            var charges = new List<Charge>
            {
                new() {
                    Quantity = inputs.MainChargeMass.ToString(CultureInfo.InvariantCulture),
                    Length = (inputs.DesignDepth - inputs.StemmingLength).ToString(CultureInfo.InvariantCulture)
                }
            };
            return charges;
        }
        /// <summary>
        /// This method generates a list of points for a Geomix mass explosion project.
        /// The points are calculated based on the corner indices of the grid.
        /// </summary>
        /// <param name="inputs">Parameters entered by the user to create a mass explosion project.</param>
        /// <param name="cosAngle">The cosine of the rotation angle.</param>
        /// <param name="sinAngle">The sine of the rotation angle.</param>
        /// <returns>A list of points representing the corners of the grid.</returns>
        private List<Point> GenerateGeomixPoints(InputParameters inputs, double cosAngle, double sinAngle)
        {
            var cornerIndices = new (int row, int col)[]
            {
                (0, 0),
                (0, inputs.MaxCol-1),
                (inputs.MaxRow - 1, inputs.MaxCol-1),
                (inputs.MaxRow-1, 0)
            };

            var points = new List<Point>(4);

            foreach (var (row, col) in cornerIndices)
            {
                var (x, y) = _coordinateCalculator.CalculateCoords(inputs, cosAngle, sinAngle, row, col);
                points.Add(new Point
                {
                    X = x.ToString(CultureInfo.InvariantCulture),
                    Y = y.ToString(CultureInfo.InvariantCulture)
                });
            }
            return points;
        }
        #endregion
    }
}