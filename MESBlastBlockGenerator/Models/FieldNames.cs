using System.Collections.Generic;

namespace MESBlastBlockGenerator.Models
{
    public static class FieldNames
    {
        public static readonly Dictionary<Fields, string> Descriptions = new()
        {
            [Fields.MaxRow] = "Количество рядов скважин",
            [Fields.MaxCol] = "Количество столбцов скважин",
            [Fields.RotationAngle] = "Угол поворота сетки скважин",
            [Fields.BaseX] = "Координата X первой скважины",
            [Fields.BaseY] = "Координата Y первой скважины",
            [Fields.BaseZ] = "Координата Z скважин",
            [Fields.Distance] = "Расстояние между скважинами",
            [Fields.PitName] = "Название карьера",
            [Fields.Level] = "Уровень проекта",
            [Fields.BlockNumber] = "Номер блока",
            [Fields.DesignDepth] = "Плановая глубина",
            [Fields.RealDepth] = "Фактическая глубина",
            [Fields.MainChargeMass] = "Масса основного заряда",
            [Fields.SecondaryChargeMass] = "Масса второго заряда",
            [Fields.StemmingLength] = "Длина забойки"
        };
    }
}
