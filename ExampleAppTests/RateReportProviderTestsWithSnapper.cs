using ExampleApp;
using Microsoft.Extensions.Options;
using Moq;
using Snapshooter.Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAppTests
{
    public class RateReportProviderTestsWithSnapper
    {
        private RateReportProvider _rateReportProvider;
        private Dictionary<string, Rate> _valute = new Dictionary<string, Rate>
        {
            ["USD"] = new Rate("USD", 100, 70),
            ["EUR"] = new Rate("EUR", 80, 75),
            ["XZ"] = new Rate("XZ", 30, 70),
        };

        public RateReportProviderTestsWithSnapper()
        {
            var rateApiClientStub = new Mock<IRateApiClient>();
            rateApiClientStub
                .Setup(c => c.GetRatesAsync())
                .ReturnsAsync(() => new RatesResponse(_valute));

            _rateReportProvider = new RateReportProvider(
                rateApiClientStub.Object,
                Mock.Of<IOptions<RateOptions>>(o => o.Value == new RateOptions { Valutes = new[] { "USD", "EUR" } }));
        }

        [Fact]
        public async Task GetValutesOrderedByDiff()
        {
            var valutes = await _rateReportProvider.GetValutes();

            Snapshot.Match(valutes);
        }

        [Fact]
        public async Task GetValutesOrderedByDiff_Projection()
        {
            var valutes = await _rateReportProvider.GetValutes();

            Snapshot.Match(valutes.Select(e => new { e.Rate }));
        }

        [Fact]
        public async Task GetValutesOrderedByDiff_Ignore()
        {
            var valutes = await _rateReportProvider.GetValutes();

            Snapshot.Match(valutes, matchOptions => matchOptions.IgnoreField("[0].Diff"));
        }

       
    }
}
