using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using arachni.Helper;
using arachni.Models;

namespace arachni.Services.Tests
{
    [TestFixture]
    public class SpiderTests
    {
        private Mock<IDocumentParser> parserMock;
        private Mock<HttpMessageHandler> handlerMock;

        [SetUp]
        public void Setup()
        {
            handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);


            parserMock = new Mock<IDocumentParser>(MockBehavior.Strict);
        }

        [Test]
        public async Task CreateSiteMapTest()
        {
            // Arrange
            const string startUrl = "https://www.domain.com/";
            var linksFoundByParser = new[] { $"{startUrl}about", $"{startUrl}contact" };
            
            const string dummyHtmlPage = "<html>I have some links, trust me!</html>";
            
            handlerMock
                .Protected()
                // Setup the PROTECTED method to mock
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(dummyHtmlPage)
                })
                .Verifiable();

            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new Uri(startUrl)
            };

            parserMock.Setup(parser => parser.GetLinksAsync(dummyHtmlPage))
                .ReturnsAsync(linksFoundByParser);


            var startingPage = new Page(startUrl);
            var targetedDomain = new Uri(startUrl);

            // Act
            var spider = new SpiderService(httpClient, new NullLogger<SpiderService>(), parserMock.Object);
            var result = await spider.GetLinkedPages(startingPage, targetedDomain, CancellationToken.None);
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(linksFoundByParser.Length, result.Count());
            Assert.That(result.Any(page => page.Url == linksFoundByParser.First()) && 
                        result.Any(page => page.Url == linksFoundByParser.Last()));


            handlerMock.Protected().Verify(
                    "SendAsync",
                    Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get
                        && req.RequestUri == new Uri(startUrl)
                    ),
                    ItExpr.IsAny<CancellationToken>()
                );
        }
    }
}