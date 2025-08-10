namespace MESBlastBlockGenerator.Models
{
    public class AppSettings
    {
        public int MaxRow { get; set; } = 100;
        public int MaxCol { get; set; } = 10;
        public double RotationAngle { get; set; } = 0;
        public double BaseX { get; set; } = 72690;
        public double BaseY { get; set; } = 98890;
        public double Distance { get; set; } = 5;
        public string PitName { get; set; } = "Верхне Нижний";
        public int Level { get; set; } = 972;
        public int BlockNumber { get; set; } = 101;
        public bool DispersedCharge { get; set; } = false;
    }
}
