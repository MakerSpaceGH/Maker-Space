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
                var response = await _http.GetStringAsync("https://worldtimeapi.org/api/timezone/Europe/Berlin");
                using var doc = JsonDocument.Parse(response);

                var datetimeStr = doc.RootElement.GetProperty("datetime").GetString();
                if (DateTimeOffset.TryParse(datetimeStr, out var netTime))
                {
                    TimeZoneInfo berlin = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
                    return TimeZoneInfo.ConvertTime(netTime.UtcDateTime, berlin);
                }
            }
            catch
            {
              
                TimeZoneInfo berlin = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, berlin);
            }

            TimeZoneInfo fallback = TimeZoneInfo.FindSystemTimeZoneById("Europe/Berlin");
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, fallback);
        }
    }
}
