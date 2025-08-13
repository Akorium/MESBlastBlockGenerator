using MESBlastBlockGenerator.DTO;
using MESBlastBlockGenerator.Models.BlastProject;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MESBlastBlockGenerator.Helpers
{
    public class MesPmvMessageGenerationHelper
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Генерирует объект типа MesPmv на основе переданных параметров.
        /// </summary>
        /// <param name="inputs">Объект типа InputParameters</param>
        /// <returns>Объект MesPmv для последующей сериализации.</returns>
        public static MesPmv GenerateMesPmvMessage(InputParameters inputs)
        {
            var mesPmv = new MesPmv
            {
                HolesInBlastProject = new HolesInBlastProject
                {
                    Holes = GenerateHoles(inputs)
                }
            };
            return mesPmv;
        }

        /// <summary>
        /// Генерирует список скважин для блока
        /// </summary>
        /// <param name="inputs">Объект типа InputParameters</param>
        /// <returns>Список объектов Hole</returns>
        private static List<Hole> GenerateHoles(InputParameters inputs)
        {
            logger.Debug("Инициализирована генерация скважин");
            string blastProjectId = Guid.NewGuid().ToString();
            double rotationAngleRad = inputs.RotationAngle * Math.PI / 180.0;
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            List<Material> materials = GenerateMaterials(inputs);
            var holes = new List<Hole>(inputs.MaxCol * inputs.MaxRow);
            for (int row = 0; row < inputs.MaxRow; row++)
            {
                for (int col = 0; col < inputs.MaxCol; col++)
                {
                    (double x, double y) = CalculateHoleCoords(inputs, cosAngle, sinAngle, row, col);

                    var hole = GenerateHole(blastProjectId, row, col, inputs, x, y, materials);
                    holes.Add(hole);
                }
            }
            return holes;
        }

        /// <summary>
        /// Генерирует объект Hole на основе переданных параметров.
        /// </summary>
        /// <param name="blastProjectId">GUID ПМВ.</param>
        /// <param name="row">Ряд скважины в ПМВ.</param>
        /// <param name="col">Столбец скважины в ПМВ.</param>
        /// <param name="inputs">Объект типа InputParameters</param>
        /// <param name="x">Координата X скважины.</param>
        /// <param name="y">Координата Y скважины.</param>
        /// <param name="materials">Список взрывчатых веществ.</param>
        /// <returns>Объект Hole, содержащий необходимые данные.</returns>
        private static Hole GenerateHole(string blastProjectId, int row, int col, InputParameters inputs, double x, double y, List<Material> materials)
        {
            string levelCode = $"{inputs.PitName}{inputs.Level}";
            string blockName = $"{inputs.Level}-{inputs.BlockNumber}";

            return new Hole
            {
                HoleItem = new()
                {
                    //MES передаёт все поля в string
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
                    X = x.ToString(CultureInfo.InvariantCulture),
                    Y = y.ToString(CultureInfo.InvariantCulture),
                    Z = inputs.BaseZ.ToString(CultureInfo.InvariantCulture),
                    XFact = x.ToString(CultureInfo.InvariantCulture),
                    YFact = y.ToString(CultureInfo.InvariantCulture),
                    ZFact = inputs.BaseZ.ToString(CultureInfo.InvariantCulture)
                },
                PlanChargeMaterials = materials
            };
        }

        /// <summary>
        /// Генерирует список взрывчатых веществ в зависимости от того явлется ли заряд рассредоточенным.
        /// </summary>
        /// <param name="dispersedCharge">Указывает, является ли заряд рассредоточенным.</param>
        /// <returns>Список объектов Material.</returns>
        private static List<Material> GenerateMaterials(InputParameters inputs)
        {
            var materials = new List<Material>();
            var exploder = new Material();
            materials.Add(exploder);

            var amounts = new List<Amount>
            {
                new() {
                    Value = inputs.MainChargeMass.ToString(CultureInfo.InvariantCulture),
                    Priority = "1"
                }
            };
            if (inputs.DispersedCharge)
            {
                amounts.Add(
                    new Amount
                    {
                        Value = inputs.SecondaryChargeMass.ToString(CultureInfo.InvariantCulture),
                        Priority = "2"
                    });
            }

            var explosive =
                new Material
                {
                    MaterialCode = "1025160",
                    MaterialShortName = "Вещество взрывчатое Березит Э-70",
                    IsExplosive = "true",
                    MaterialDensity = "1200",
                    CupDensity = "0",
                    Amounts = amounts
                };
            materials.Add(explosive);
            return materials;
        }

        /// <summary>
        /// Расчитывает координаты скважины
        /// </summary>
        /// <param name="inputs">Объект типа InputParameters</param>
        /// <param name="cosAngle">Косинус угла поворота блока</param>
        /// <param name="sinAngle">Синус угла поворота блока</param>
        /// <param name="row">Ряд скважины</param>
        /// <param name="col">Столбец скважины</param>
        /// <returns>Координаты X и Y</returns>
        private static (double x, double y) CalculateHoleCoords(InputParameters inputs, double cosAngle, double sinAngle, int row, int col)
        {
            double x = inputs.BaseX + col * inputs.Distance;
            double y = inputs.BaseY + row * inputs.Distance;

            double relX = x - inputs.BaseX;
            double relY = y - inputs.BaseY;

            x = inputs.BaseX + relX * cosAngle - relY * sinAngle;
            y = inputs.BaseY + relX * sinAngle + relY * cosAngle;

            return (x, y);
        }
    }
}