using MESBlastBlockGenerator.Models.MESBlastProject;

namespace MESBlastBlockGenerator.Models.MicromineBlastProject
{
    public class IntervalRecord
    {
        public string Hole { get; set; }
        public string HoleType { get; set; } = "Запланировано";
        public string Block { get; set; }
        public double From { get; set; } = 0;
        public double To { get; set; }
        public string IntervalType { get; set; } = "Забойка";
        public double ChargeDensity { get; set; } = 1;
        public double? ChargeLength { get; set; } = null;
        public double? ChargeDiameter { get; set; } = null;
        public double? ExplosiveWeigth { get; set; } = null;
    }
}
