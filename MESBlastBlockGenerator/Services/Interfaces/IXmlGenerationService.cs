using MESBlastBlockGenerator.Models;
using System.Threading.Tasks;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface IXmlGenerationService
    {
        Task<string> GenerateXmlContentAsync(InputParameters inputs);
    }
}
