using MESBlastBlockGenerator.Models.SOAP;
using MESBlastBlockGenerator.Models.SOAP.Response;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MESBlastBlockGenerator.Services
{
    public class SoapClientService : ISoapClientService
    {
        private readonly IXmlSerializationService _serializationService;
        private readonly HttpClient _httpClient;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public SoapClientService(IXmlSerializationService serializationService)
        {
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            _serializationService = serializationService;
        }

        public async Task<bool> SendXmlAsync(string xmlContent, string endpointUrl)
        {
            try
            {
                var response = await SendXmlWithResponseAsync(xmlContent, endpointUrl);
                return response != null && IsSuccessResponse(response);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка отправки XML через SOAP сервис");
                return false;
            }
        }

        public async Task<Envelope<ResponseBody>?> SendXmlWithResponseAsync(string xmlContent, string endpointUrl)
        {
            try
            {
                var content = new StringContent(xmlContent, Encoding.UTF8, "application/xml");
                var response = await _httpClient.PostAsync(endpointUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return ParseSoapResponse(responseContent);
                }

                _logger.Error($"Ошибка HTTP при отправке SOAP: {response.StatusCode}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка отправки SOAP запроса");
                return null;
            }
        }

        private Envelope<ResponseBody>? ParseSoapResponse(string xmlResponse)
        {
            try
            {
                return _serializationService.Deserialize<Envelope<ResponseBody>>(xmlResponse);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка десериализации SOAP ответа");
                return null;
            }
        }

        private bool IsSuccessResponse(Envelope<ResponseBody> response)
        {
            try
            {
                var dto = response?.Body?.SoapXmlRequestResponse?.XmlResponse?.AsuSzmInSoapResponseDto;

                if (dto == null)
                    return false;

                var hasSuccessStatus = dto.Status?.ToLower() == "true";
                var hasSuccessError = dto.Error?.Contains("Status code: 200") == true;

                return hasSuccessStatus && hasSuccessError;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Ошибка проверки статуса SOAP ответа");
                return false;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}
