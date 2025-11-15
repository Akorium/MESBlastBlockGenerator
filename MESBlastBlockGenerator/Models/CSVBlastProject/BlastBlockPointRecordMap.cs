using CsvHelper.Configuration;

namespace MESBlastBlockGenerator.Models.CSVBlastProject
{
    public class BlastBlockPointRecordMap : ClassMap<BlastBlockPointRecord>
    {
        public BlastBlockPointRecordMap()
        {
            Map(m => m.BlastBlockName).Name("blast_block/name");
            Map(m => m.Sequence).Name("sequence");
            Map(m => m.X).Name("x");
            Map(m => m.Y).Name("y");
            Map(m => m.Z).Name("z");
        }
    }
}
