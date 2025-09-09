using System.Threading.Tasks;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface IJsonSerializationService
    {
        T DeserializeFromFile<T>(string filePath) where T : class, new();

        void SerializeToFile<T>(T obj, string filePath);
    }
}