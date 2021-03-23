using Microsoft.Extensions.Options;
using System.Linq;
using System.Threading.Tasks;

namespace ExampleApp
{
    public class App
    {
        private readonly IRateApiClient _rateApiClient;
        private readonly INotificationSender _notificationSender;
        private readonly RateOptions _rateOptions;

        public App(IRateApiClient rateApiClient, INotificationSender notificationSender, IOptions<RateOptions> rateOptions) 
        {
            _rateApiClient = rateApiClient;
            _notificationSender = notificationSender;
            _rateOptions = rateOptions.Value;
        }

        public async Task Run()
        {
            var rates = await _rateApiClient.GetRatesAsync();

            var valutes = rates.Valute
                .Where(v => _rateOptions.Valutes.Contains(v.Key))
                .Select(v =>
                {
                    var diff = (v.Value.Previous - v.Value.Value);
                    return (v.Key, v.Value.Value, diff);
                })
                .OrderByDescending(v => v.diff);

            var notificationContent = "Курсы на сегодня <br/>" + string.Join("<br/>", valutes);

            _notificationSender.Send(notificationContent);

        }
    }
}
