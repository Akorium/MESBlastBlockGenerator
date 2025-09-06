namespace MESBlastBlockGenerator.Models
{
    public class AppSettings
    {
        public string MaxRow { get; set; } = "100";
        public string MaxCol { get; set; } = "10";
        public string RotationAngle { get; set; } = "0";
        public string BaseX { get; set; } = "72690";
        public string BaseY { get; set; } = "98890";
        public string BaseZ { get; set; } = "980,66";
        public string Distance { get; set; } = "5";
        public string PitName { get; set; } = "Верхне Нижний";
        public string Level { get; set; } = "972";
        public string BlockNumber { get; set; } = "101";
        public string DesignDepth { get; set; } = "9,5";
        public string RealDepth { get; set; } = "7";
        public bool DispersedCharge { get; set; } = false;
        public string MainChargeMass { get; set; } = "500";
        public string SecondaryChargeMass { get; set; } = "600";
        public string StemmingLength { get; set; } = "4,59";
    }
}
