using ExampleApp;
using Microsoft.Extensions.Options;
using Moq;
using Snapshooter.Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAppTests
{

    public class AppTestsWithSnapper
    {
        private readonly App _app;
        private string _notification;
        private Dictionary<string, Rate> _valute = new Dictionary<string, Rate>
        {
           ["USD"] = new Rate("USD", 100, 70),
           ["EUR"] = new Rate("EUR", 80, 75),
           ["XZ"] = new Rate("XZ", 30, 70),
        };

        public AppTestsWithSnapper()
        {
            var rateApiClientStub = new Mock<IRateApiClient>();
            rateApiClientStub
                .Setup(c => c.GetRatesAsync())
                .ReturnsAsync(() => new RatesResponse(_valute));

            var notificationSenderMock = new Mock<INotificationSender>();
            notificationSenderMock
                .Setup(s => s.Send(It.IsAny<string>()))
                .Callback<string>(n => _notification = n);

            _app = new App(
                rateApiClientStub.Object,
                notificationSenderMock.Object,
                Mock.Of<IOptions<RateOptions>>(o => o.Value == new RateOptions { Valutes = new[] { "USD", "EUR" }})); 
        }

        [Fact]
        public async Task SendValutesOrderedByDiff()
        {            
            await _app.Run();

            Snapshot.Match(_notification);
        }
    }
}
