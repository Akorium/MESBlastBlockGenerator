using CsvHelper.Configuration;

namespace MESBlastBlockGenerator.Models.MicromineBlastProject
{
    public class IntervalRecordMap : ClassMap<IntervalRecord>
    {
        public IntervalRecordMap()
        {
            Map(m => m.Hole).Name("HOLE");
            Map(m => m.HoleType).Name("HOLE_TYPE");
            Map(m => m.Block).Name("BLOCK");
            Map(m => m.From).Name("FROM");
            Map(m => m.To).Name("TO");
            Map(m => m.IntervalType).Name("INTERVAL TYPE");
            Map(m => m.ChargeDensity).Name("CHARGE DENSITY");
            Map(m => m.ChargeLength).Name("CHARGE LENGTH");
            Map(m => m.ChargeDiameter).Name("CHARGE DIAMETER");
            Map(m => m.ExplosiveWeigth).Name("EXPLOSIVE WEIGHT");
        }
    }
}
