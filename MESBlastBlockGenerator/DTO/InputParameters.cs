namespace MESBlastBlockGenerator.DTO
{
    public class InputParameters
    {
        public int MaxRow { get; set; }
        public int MaxCol { get; set; }
        public double RotationAngle { get; set; }
        public double BaseX { get; set; }
        public double BaseY { get; set; }
        public double Distance { get; set; }
        public string PitName { get; set; } = "";
        public int Level { get; set; }
        public int BlockNumber { get; set; }
        public bool DispersedCharge { get; set; }
    }
}
