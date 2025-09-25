using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace My_own_website.Services
{
    public class TimeService
    {
        private readonly HttpClient _http;

        public TimeService(HttpClient http)
        {
            _http = http;
        }

        public async Task<DateTime> GetNetworkTimeAsync()
        {
            try
            {
                // Weltzeit API
                var response = await _http.GetStringAsync("http://worldtimeapi.org/api/timezone/Europe/Berlin");
                using var doc = JsonDocument.Parse(response);
                var datetime = doc.RootElement.GetProperty("datetime").GetString();
                if (DateTime.TryParse(datetime, out var netTime))
                {
                    return netTime;
                }
            }
            catch
            {
                // Falls API fehlschlägt, fallback auf Serverzeit
                return DateTime.Now;
            }

            return DateTime.Now;
        }
    }
}
