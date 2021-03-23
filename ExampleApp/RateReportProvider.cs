using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApp
{
    public class RateReportProvider
    {
        private readonly IRateApiClient _rateApiClient;
        private readonly RateOptions _rateOptions;

        public RateReportProvider(IRateApiClient rateApiClient, IOptions<RateOptions> rateOptions) 
        {
            _rateApiClient = rateApiClient;
            _rateOptions = rateOptions.Value;
        }

        public record ReportRow(string Valute, decimal Rate, decimal Diff);

        public async Task<ReportRow[]> GetValutes()
        {
            var rates = await _rateApiClient.GetRatesAsync();

            return rates.Valute
                .Where(v => _rateOptions.Valutes.Contains(v.Key))
                .Select(v =>
                {
                    var diff = (v.Value.Previous - v.Value.Value);
                    return new ReportRow(v.Key, v.Value.Value, diff);
                })
                .OrderByDescending(v => v.Diff)
                .ToArray();
        }
    }
}
