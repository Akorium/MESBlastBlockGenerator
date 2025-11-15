namespace MESBlastBlockGenerator.Models.CSVBlastProject
{
    public class BlastBlockPointRecord
    {
        public string BlastBlockName { get; set; } = "";
        public int Sequence { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; } = 0;
    }
}
