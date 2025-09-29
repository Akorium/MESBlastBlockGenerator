using MESBlastBlockGenerator.Models.SOAP;
using MESBlastBlockGenerator.Models.SOAP.Response;
using System.Threading.Tasks;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface ISoapClientService
    {
        Task<bool> SendXmlAsync(string xmlContent);
    }
}
