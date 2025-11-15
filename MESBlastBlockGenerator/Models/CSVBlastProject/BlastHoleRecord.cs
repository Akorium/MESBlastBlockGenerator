using System;

namespace MESBlastBlockGenerator.Models.CSVBlastProject
{
    public class BlastHoleRecord
    {
        public string Name { get; set; } = "";
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public string BlastBlockName { get; set; } = "";
        public string BlastBlockBlastedDate { get; set; } = DateTime.Now.AddMonths(1).ToString("dd.MM.yyyy");
        public double DesignChargeMass { get; set; }
        public double DesignChargeHeight { get; set; }
        public string DesignExplosiveName { get; set; } = "Тип ВВ 1";
        public double Depth { get; set; }
        public double Diameter { get; set; }
        public double Tamping { get; set; }
    }
}
