using MESBlastBlockGenerator.Enums;

namespace MESBlastBlockGenerator.Models
{
    public class InputParameters()
    {
        public int MaxRow { get; set; } = 100;
        public int MaxCol { get; set; } = 10;
        public double RotationAngle { get; set; } = 0;
        public double BaseX { get; set; } = 72690;
        public double BaseY { get; set; } = 98890;
        public double BaseZ { get; set; } = 980.66;
        public double Distance { get; set; } = 5;
        public string PitName { get; set; } = "Верхне Нижний";
        public int Level { get; set; } = 972;
        public int BlockNumber { get; set; } = 101;
        public double DesignDepth { get; set; } = 9.5;
        public double RealDepth { get; set; } = 7;
        public bool DispersedCharge { get; set; } = false;
        public double MainChargeMass { get; set; } = 500;
        public double SecondaryChargeMass { get; set; } = 600;
        public double StemmingLength { get; set; } = 4.59;
        public int DesignDiameter { get; set; } = 233;
        public int RealDiameter { get; set; } = 233;
        public bool IsDrilling { get; set; } = false;
        public double CoordinatesDeviation { get; set; } = 0;
        public HoleMaterialType HoleMaterialType  { get; set; } = HoleMaterialType.ВСДП;
    }
}
