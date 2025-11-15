using MESBlastBlockGenerator.Models;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface IGenerationService
    {
        string GenerateMESMassExplosionProject(InputParameters inputs);
        string GenerateGeomixMassExplosionProject(InputParameters inputs);
        (string blastHoles, string blastBlockPoints) GenerateBlastProjectCsv(InputParameters inputs);
    }
}
