using CsvHelper.Configuration;

namespace MESBlastBlockGenerator.Models.MicromineBlastProject
{
    public class CollarRecordMap : ClassMap<CollarRecord>
    {
        public CollarRecordMap()
        {
            Map(m => m.Hole).Name("HOLE");
            Map(m => m.HoleType).Name("HOLE_TYPE");
            Map(m => m.Block).Name("BLOCK");
            Map(m => m.East).Name("EAST");
            Map(m => m.North).Name("NORTH");
            Map(m => m.Rl).Name("RL");
            Map(m => m.Dip).Name("DIP");
            Map(m => m.Azimuth).Name("AZIMUTH");
            Map(m => m.Depth).Name("DEPTH");
            Map(m => m.Row).Name("ROW");
            Map(m => m.HoleDiameter).Name("HOLE DIAM");
            Map(m => m.Subdrill).Name("SUBDRILL");
            Map(m => m.FiringSequence).Name("FIRING_SEQUENCE");
            Map(m => m.FiringDelay).Name("FIRING_DELAY");
            Map(m => m.Spacing).Name("SPACING");
            Map(m => m.Burden).Name("BURDEN");
        }
    }
}
