using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using arachni.Managers;
using arachni.Models;
using arachni.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace arachni.Controllers.Tests
{
    [TestFixture]
    public class CrawlControllerTests
    {
        private Mock<ISpiderManager> spiderManagerMock;
        
        [SetUp]
        public void Setup()
        {
            spiderManagerMock = new Mock<ISpiderManager>();
        }

        [Test]
        public async Task GetTest()
        {
            // Arrange
            const int reqTimeout = 20;
            const string reqDomain = "https://sub.domain.com";
            var reqUrl = $"{reqDomain}/firstpage";

            var expectedPages = new[] {new PageDto($"{reqUrl}/about", new ConcurrentDictionary<string, short>()), new PageDto(reqUrl, new ConcurrentDictionary<string, short>())};

            spiderManagerMock.Setup(spider => spider.CreateSiteMap(reqUrl, It.IsAny<CancellationToken>())).ReturnsAsync(expectedPages);

            var controller = new CrawlController(new NullLogger<CrawlController>(), spiderManagerMock.Object);

            // Act
            var result = (await controller.Post(reqUrl, reqTimeout)).Result as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var dto = result.Value as SiteMap;
            Assert.AreEqual(reqUrl, dto.StartUrl);
            Assert.AreEqual(reqDomain, dto.RequestedDomain);
            Assert.IsTrue(dto.Completed);
            Assert.AreEqual(expectedPages, dto.Pages);
        }

        [Test]
        [TestCase("/index")]
        [TestCase(null)]
        public async Task GetTestBadRequest(string reqUrl)
        {
            // Arrange
            const int reqTimeout = 20;

            var controller = new CrawlController(new NullLogger<CrawlController>(), spiderManagerMock.Object);

            // Act
            var result = await controller.Post(reqUrl, reqTimeout);

            // Assert
            Assert.That(result.Result is BadRequestResult);
        }
    }
}