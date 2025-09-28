using MESBlastBlockGenerator.Models;
using System.Threading.Tasks;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface IXmlGenerationService
    {
        string GenerateMESMassExplosionProject(InputParameters inputs);
        string GenerateGeomixMassExplosionProject(InputParameters inputs);
    }
}
