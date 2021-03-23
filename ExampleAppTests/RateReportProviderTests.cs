using ExampleApp;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAppTests
{
    public class RateReportProviderTests
    {
        private RateReportProvider _rateReportProvider;
        private Dictionary<string, Rate> _valute = new Dictionary<string, Rate>
        {
            ["USD"] = new Rate("USD", 100, 70),
            ["EUR"] = new Rate("EUR", 80, 75),
            ["XZ"] = new Rate("XZ", 30, 70),
        };

        public RateReportProviderTests()
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

            valutes.Should().BeEquivalentTo(new[] { 
                new { Valute = "EUR" }, 
                new { Valute = "USD" } 
            }, options => options.WithStrictOrdering());
        }
    }
}
