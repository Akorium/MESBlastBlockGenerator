using CsvHelper.Configuration;

namespace MESBlastBlockGenerator.Models.CSVBlastProject
{
    public class BlastHoleRecordMap : ClassMap<BlastHoleRecord>
    {
        public BlastHoleRecordMap()
        {
            Map(m => m.Name).Name("name");
            Map(m => m.X).Name("x");
            Map(m => m.Y).Name("y");
            Map(m => m.Z).Name("z");
            Map(m => m.BlastBlockName).Name("blast_block/name");
            Map(m => m.BlastBlockBlastedDate).Name("blast_block/blasted_date");
            Map(m => m.DesignChargeMass).Name("design_charge_mass");
            Map(m => m.DesignChargeHeight).Name("design_charge_height");
            Map(m => m.DesignExplosiveName).Name("design_explosive_name");
            Map(m => m.Depth).Name("depth");
            Map(m => m.Diameter).Name("diameter");
            Map(m => m.Tamping).Name("tamping");
        }
    }
}
