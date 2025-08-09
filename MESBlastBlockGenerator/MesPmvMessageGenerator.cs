using MESBlastBlockGenerator.Models.BlastProject;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace MESBlastBlockGenerator
{
    public class MesPmvMessageGenerator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Генерирует объект типа MesPmv на основе переданных параметров.
        /// </summary>
        /// <param name="maxRow">Количество рядов скважин в ПМВ.</param>
        /// <param name="maxCol">Количество столбцов скважин в ПМВ.</param>
        /// <param name="rotationAngleDegrees">Угол поворота сетки скважин в ПМВ.</param>
        /// <param name="baseX">X-координата первой скважины.</param>
        /// <param name="baseY">Y-координата первой скважины.</param>
        /// <param name="distance">Расстояние между скважинами.</param>
        /// <param name="pitName">Название карьера.</param>
        /// <param name="level">Уровень проекта.</param>
        /// <param name="blockNumber">Номер блока.</param>
        /// <param name="dispersedCharge">Является ли заряд рассредоточенным.</param>
        /// <returns>Объект MesPmv для последующей сериализации.</returns>
        public static MesPmv GenerateMesPmvMessage(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance, string pitName, int level, int blockNumber, bool dispersedCharge)
        {
            var mesPmv = new MesPmv
            {
                HolesInBlastProject = new HolesInBlastProject
                {
                    Holes = GenerateHoles(maxRow, maxCol, rotationAngleDegrees, baseX, baseY, distance, pitName, level, blockNumber, dispersedCharge)
                }
            };
            return mesPmv;
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
        /// /// <param name="dispersedCharge">Является ли заряд рассредоточенным.</param>
        /// <returns>Список объектов Hole</returns>
        private static List<Hole> GenerateHoles(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance, string pitName, int level, int blockNumber, bool dispersedCharge)
        {
            logger.Debug("Инициализирована генерация скважин");
            string blastProjectId = Guid.NewGuid().ToString();
            double rotationAngleRad = rotationAngleDegrees * Math.PI / 180.0;
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            List<Material> materials = GenerateMaterials(dispersedCharge);
            var holes = new List<Hole>(maxRow * maxCol);
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    (double x, double y) = CalculateHoleCoords(baseX, baseY, distance, cosAngle, sinAngle, row, col);

                    var hole = GenerateHole(blastProjectId, row, col, pitName, level, blockNumber, x, y, materials);
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
        /// <param name="pitName">Название карьера.</param>
        /// <param name="level">Уровень проекта.</param>
        /// <param name="blockNumber">Номер блока.</param>
        /// <param name="x">Координата X скважины.</param>
        /// <param name="y">Координата Y скважины.</param>
        /// <param name="materials">Список взрывчатых веществ.</param>
        /// <returns>Объект Hole, содержащий необходимые данные.</returns>
        private static Hole GenerateHole(string blastProjectId, int row, int col, string pitName, int level, int blockNumber, double x, double y, List<Material> materials)
        {
            string levelCode = $"{pitName}{level}";
            string blockName = $"{level}-{blockNumber}";

            return new Hole
            {
                HoleItem = new()
                {
                    //MES передаёт все поля в string
                    BlastProjectId = blastProjectId,
                    HoleId = Guid.NewGuid().ToString(),
                    HoleNumber = $"{row:D2}{col:D2}",
                    PitCode = pitName,
                    PitName = pitName,
                    LevelCode = $"{levelCode}",
                    LevelName = $"{level}",
                    BlockCode = $"{levelCode}-{blockNumber}",
                    BlockName = $"{blockName}",
                    BlockDrillingCode = $"{pitName}{levelCode}{blockName}Drill",
                    BlockDrillingName = $"{blockName}",
                    BlockBlastingCode = $"{pitName}{blockName}Blast",
                    BlockBlastingName = $"{blockName}",
                    X = x.ToString(CultureInfo.InvariantCulture),
                    Y = y.ToString(CultureInfo.InvariantCulture),
                    XFact = x.ToString(CultureInfo.InvariantCulture),
                    YFact = y.ToString(CultureInfo.InvariantCulture)
                },
                PlanChargeMaterials = materials
            };
        }

        /// <summary>
        /// Генерирует список взрывчатых веществ в зависимости от того явлется ли заряд рассредоточенным.
        /// </summary>
        /// <param name="dispersedCharge">Указывает, является ли заряд рассредоточенным.</param>
        /// <returns>Список объектов Material.</returns>
        private static List<Material> GenerateMaterials(bool dispersedCharge)
        {
            var materials = new List<Material>();
            var exploder = new Material();
            materials.Add(exploder);

            var amounts = new List<Amount>
            {
                new() {
                    Value = "500",
                    Priority = "1"
                }
            };
            if (dispersedCharge)
            {
                amounts.Add(
                    new Amount
                    {
                        Value = "600",
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