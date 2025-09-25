using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Models.BlastProject;
using MESBlastBlockGenerator.Models.GeomixBlastProject;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MESBlastBlockGenerator.Helpers
{
    /// <summary>
    /// Helper class for generating MES PMV messages.
    /// </summary>
    public class MesPmvMessageGenerationHelper
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Generates a MES PMV message based on the provided input parameters.
        /// </summary>
        /// <param name="inputs">Input parameters for generating the MES PMV message.</param>
        /// <returns>A MES PMV message.</returns>
        public static MesPmv GenerateMesPmvMessage(InputParameters inputs)
        {
            // Create a new MES PMV message
            var mesPmv = new MesPmv
            {
                HolesInBlastProject = new HolesInBlastProject
                {
                    // Generate holes for the blast project
                    Holes = GenerateHoles(inputs)
                }
            };
            return mesPmv;
        }
        public static GeomixBlastProjects GenerateGeomixBlastProjects(InputParameters inputs)
        {
            var projectId = $"{inputs.Level}-{inputs.BlockNumber}";
            var blastProjectPoints = GenerateBlastProjectPoints(inputs);
            var wells = GenerateWells(inputs);

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

            return new GeomixBlastProjects
            {
                ProjectList = [project]
            };
        }

        private static List<Well> GenerateWells(InputParameters inputs)
        {
            _logger.Debug("Инициализирована генерация скважин");

            // Calculate rotation angle in radians
            double rotationAngleRad = inputs.RotationAngle * Math.PI / 180.0;

            // Calculate cosine and sine of the rotation angle
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            // Generate materials for the blast project
            List<Charge> charge = GenerateCharge(inputs);

            // Initialize list of holes
            var wells = new List<Well>(inputs.MaxCol * inputs.MaxRow);

            // Iterate over rows and columns to generate holes
            for (int row = 0; row < inputs.MaxRow; row++)
            {
                for (int col = 0; col < inputs.MaxCol; col++)
                {
                    // Calculate hole coordinates
                    (double x, double y) = CalculateCoords(inputs, cosAngle, sinAngle, row, col);

                    // Generate a hole
                    var well = GenerateWell(row, col, inputs, x, y, charge);

                    // Add hole to the list
                    wells.Add(well);
                }
            }
            return wells;
        }

        private static Well GenerateWell(int row, int col, InputParameters inputs, double x, double y, List<Charge> charge)
        {
            var well = new Well
            {
                WellID = $"{row:D2}{col:D2}",
                WellNumber = $"{row:D2}{col:D2}",
                Depth = inputs.DesignDepth.ToString(CultureInfo.InvariantCulture),
                X = x.ToString(CultureInfo.InvariantCulture),
                Y = y.ToString(CultureInfo.InvariantCulture),
                Z = inputs.BaseZ.ToString(CultureInfo.InvariantCulture),
                DM = inputs.DesignDiameter.ToString(CultureInfo.InvariantCulture),
                Charges = new Charges 
                {
                    ChargeList = charge
                }
            };
            return well;
        }

        private static List<Charge> GenerateCharge(InputParameters inputs)
        {
            var charges = new List<Charge>();
            var charge = new Charge
            {
                Quantity = inputs.MainChargeMass.ToString(CultureInfo.InvariantCulture),
                Length = (inputs.DesignDepth - inputs.StemmingLength).ToString(CultureInfo.InvariantCulture)
            };
            charges.Add(charge);
            return charges;
        }

        private static List<Point> GenerateBlastProjectPoints(InputParameters inputs)
        {
            var points = new List<Point>();
            // Calculate rotation angle in radians
            double rotationAngleRad = inputs.RotationAngle * Math.PI / 180.0;

            // Calculate cosine and sine of the rotation angle
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            var cornerIndices = new (int row, int col)[]
            {
                (0, 0),
                (0, inputs.MaxCol), 
                (inputs.MaxRow, 0),
                (inputs.MaxRow, inputs.MaxCol)
            };

            foreach (var (row, col) in cornerIndices)
            {
                (double x, double y) = CalculateCoords(inputs, cosAngle, sinAngle, row, col);
                points.Add(new Point { X = x.ToString(), Y = y.ToString() });
            }
            return points;
        }

        /// <summary>
        /// Generates a list of holes for the blast project.
        /// </summary>
        /// <param name="inputs">Input parameters for generating holes.</param>
        /// <returns>A list of holes.</returns>
        private static List<Hole> GenerateHoles(InputParameters inputs)
        {
            _logger.Debug("Инициализирована генерация скважин");

            // Generate a unique blast project ID
            string blastProjectId = Guid.NewGuid().ToString();

            // Calculate rotation angle in radians
            double rotationAngleRad = inputs.RotationAngle * Math.PI / 180.0;

            // Calculate cosine and sine of the rotation angle
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            // Generate materials for the blast project
            List<MESBlastBlockGenerator.Models.BlastProject.Material> materials = GenerateMaterials(inputs);

            // Initialize list of holes
            var holes = new List<Hole>(inputs.MaxCol * inputs.MaxRow);

            // Iterate over rows and columns to generate holes
            for (int row = 0; row < inputs.MaxRow; row++)
            {
                for (int col = 0; col < inputs.MaxCol; col++)
                {
                    // Calculate hole coordinates
                    (double x, double y) = CalculateCoords(inputs, cosAngle, sinAngle, row, col);

                    // Generate a hole
                    var hole = GenerateHole(blastProjectId, row, col, inputs, x, y, materials);

                    // Add hole to the list
                    holes.Add(hole);
                }
            }
            return holes;
        }

        /// <summary>
        /// Generates a hole based on the provided parameters.
        /// </summary>
        /// <param name="blastProjectId">Unique ID of the blast project.</param>
        /// <param name="row">Row number of the hole.</param>
        /// <param name="col">Column number of the hole.</param>
        /// <param name="inputs">Input parameters for generating the hole.</param>
        /// <param name="x">X-coordinate of the hole.</param>
        /// <param name="y">Y-coordinate of the hole.</param>
        /// <param name="materials">Materials used in the hole.</param>
        /// <returns>A hole.</returns>
        private static Hole GenerateHole(string blastProjectId, int row, int col, InputParameters inputs, double x, double y, List<MESBlastBlockGenerator.Models.BlastProject.Material> materials)
        {
            // Generate level code and block name
            string levelCode = $"{inputs.PitName}{inputs.Level}";
            string blockName = $"{inputs.Level}-{inputs.BlockNumber}";

            // Create a new hole
            var hole = new Hole
            {
                // All fields in string with invariant culture because that is how MES sends them
                HoleItem = new()
                {
                    BlastProjectId = blastProjectId,
                    HoleId = Guid.NewGuid().ToString(),
                    HoleNumber = $"{row:D2}{col:D2}",
                    PitCode = inputs.PitName,
                    PitName = inputs.PitName,
                    LevelCode = $"{levelCode}",
                    LevelName = $"{inputs.Level}",
                    BlockCode = $"{levelCode}-{inputs.BlockNumber}",
                    BlockName = $"{blockName}",
                    BlockDrillingCode = $"{inputs.PitName}{levelCode}{blockName}Drill",
                    BlockDrillingName = $"{blockName}",
                    BlockBlastingCode = $"{inputs.PitName}{blockName}Blast",
                    BlockBlastingName = $"{blockName}",
                    DepthPlan = inputs.DesignDepth.ToString(CultureInfo.InvariantCulture),
                    DiameterPlan = inputs.DesignDiameter.ToString(CultureInfo.InvariantCulture),
                    X = x.ToString(CultureInfo.InvariantCulture),
                    Y = y.ToString(CultureInfo.InvariantCulture),
                    Z = inputs.BaseZ.ToString(CultureInfo.InvariantCulture),
                    IsDrilling = inputs.IsDrilling.ToString().ToLower()
                    
                },
                PlanChargeMaterials = materials,
                StemmingLengthPlan = new()
                {
                    Value = inputs.StemmingLength.ToString(CultureInfo.InvariantCulture)
                }
            };
            if (inputs.IsDrilling)
            {
                hole.HoleItem.DepthFact = inputs.RealDepth.ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.DepthFactEomId = hole.HoleItem.DepthPlanEomId;
                hole.HoleItem.DepthFactEom = hole.HoleItem.DepthPlanEom;
                hole.HoleItem.DiameterFact = inputs.RealDiameter.ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.DiameterFactEomId = hole.HoleItem.DiameterEomId;
                hole.HoleItem.DiameterFactEom = hole.HoleItem.DiameterEom;
                hole.HoleItem.XFact = (x + inputs.CoordinatesDeviation).ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.YFact = (y - inputs.CoordinatesDeviation).ToString(CultureInfo.InvariantCulture);
                hole.HoleItem.ZFact = (inputs.BaseZ + inputs.CoordinatesDeviation).ToString(CultureInfo.InvariantCulture);
            }
            return hole;
        }

        /// <summary>
        /// Generates a list of materials for the blast project.
        /// </summary>
        /// <param name="inputs">Input parameters for generating materials.</param>
        /// <returns>A list of materials.</returns>
        private static List<MESBlastBlockGenerator.Models.BlastProject.Material> GenerateMaterials(InputParameters inputs)
        {
            // Initialize list of materials
            var materials = new List<MESBlastBlockGenerator.Models.BlastProject.Material>();

            // Add exploder material
            var exploder = new MESBlastBlockGenerator.Models.BlastProject.Material();
            materials.Add(exploder);

            // Initialize list of amounts
            var amounts = new List<Amount>
            {
                new() {
                    Value = inputs.MainChargeMass.ToString(CultureInfo.InvariantCulture)
                }
            };

            // Add secondary charge amount if dispersed charge is true
            if (inputs.DispersedCharge)
            {
                amounts.Add(
                    new Amount
                    {
                        Value = inputs.SecondaryChargeMass.ToString(CultureInfo.InvariantCulture),
                        Priority = "2"
                    });
            }

            // Create explosive material
            var explosive =
                new MESBlastBlockGenerator.Models.BlastProject.Material
                {
                    MaterialCode = "1025160",
                    MaterialShortName = "Вещество взрывчатое Березит Э-70",
                    IsExplosive = "true",
                    MaterialDensity = "1200",
                    CupDensity = "0",
                    Amounts = amounts
                };

            // Add explosive material to the list
            materials.Add(explosive);

            return materials;
        }

        /// <summary>
        /// Calculates the coordinates of a hole based on the provided parameters.
        /// </summary>
        /// <param name="inputs">Input parameters for calculating hole coordinates.</param>
        /// <param name="cosAngle">Cosine of the rotation angle.</param>
        /// <param name="sinAngle">Sine of the rotation angle.</param>
        /// <param name="row">Row number of the hole.</param>
        /// <param name="col">Column number of the hole.</param>
        /// <returns>The coordinates of the hole.</returns>
        private static (double x, double y) CalculateCoords(InputParameters inputs, double cosAngle, double sinAngle, int row, int col)
        {
            // Calculate initial x and y coordinates based on row and column numbers
            double x = inputs.BaseX + col * inputs.Distance;
            double y = inputs.BaseY - row * inputs.Distance;

            // Calculate relative x and y coordinates with respect to the base point
            double relX = x - inputs.BaseX;
            double relY = y - inputs.BaseY;

            // Apply rotation transformation to calculate final x and y coordinates
            x = inputs.BaseX + relX * cosAngle - relY * sinAngle;
            y = inputs.BaseY - relX * sinAngle + relY * cosAngle;

            // Return the calculated x and y coordinates as a tuple
            return (x, y);
        }
    }
}