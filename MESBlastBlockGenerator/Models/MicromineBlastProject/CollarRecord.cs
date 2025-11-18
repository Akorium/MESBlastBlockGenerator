namespace MESBlastBlockGenerator.Models.MicromineBlastProject
{
    public class CollarRecord
    {
        public string Hole { get; set; }
        public string HoleType { get; set; } = "Запланировано";
        public string Block { get; set; }
        public double East { get; set; }
        public double North { get; set; }
        public double Rl {  get; set; }
        public double Dip { get; set; } = -90;
        public double Azimuth { get; set; } = 0;
        public double Depth { get; set; }
        public int Row { get; set; }
        public int HoleDiameter { get; set; }
        public double Subdrill { get; set; } = 0;
        public double? FiringSequence { get; set; } = null;
        public double? FiringDelay { get; set; } = null;
        public double Spacing { get; set; }
        public double Burden { get; set; }
    }
}
