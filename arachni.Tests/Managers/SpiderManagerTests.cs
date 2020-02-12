using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using arachni.Models;
using arachni.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace arachni.Managers.Tests
{
    [TestFixture]
    public class SpiderManagerTests
    {
        private Mock<ISpiderService> spiderMock;

        private const string startUrl = "https://www.domain.com/";
        private readonly string[] linksFoundByParser = { $"{startUrl}about", $"{startUrl}contact" };

        [SetUp]
        public void Setup()
        {
            spiderMock = new Mock<ISpiderService>(MockBehavior.Strict);

        }

        [Test]
        public async Task CreateSiteMapTest()
        {
            // Arrange
            CancellationToken token = new CancellationToken(false);

            spiderMock.Setup(service => service.GetLinkedPages(It.IsAny<Page>(), It.IsAny<Uri>(), token))
                .ReturnsAsync(linksFoundByParser.Select(link => new Page(link, startUrl)));

            // Act 
            var manager = new SpiderManager(new NullLogger<SpiderService>(), spiderMock.Object);
            var result = await manager.CreateSiteMap(startUrl, token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());
            Assert.That(result.Any(dto => dto.Url == linksFoundByParser.First()));
            Assert.That(result.Any(dto => dto.Url == linksFoundByParser.Last()));
            Assert.That(result.Any(dto => dto.Url == startUrl));
        }

        [Test]
        public async Task CreateSiteMapReturnIncompleteSitemapWhenCancellationTokenIsFlipped()
        {
            // Arrange
            CancellationToken token = new CancellationToken(true);

            spiderMock.Setup(service => service.GetLinkedPages(It.IsAny<Page>(), It.IsAny<Uri>(), token))
                .ReturnsAsync(new List<Page>());

            // Act 

            var manager = new SpiderManager(new NullLogger<SpiderService>(), spiderMock.Object);
            var result = await manager.CreateSiteMap(startUrl, token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(startUrl, result.First().Url);
        }

        [Test]
        public async Task CreateSiteMapAssignTheCorrectParentPage()
        {
            // Arrange
            CancellationToken token = new CancellationToken(false);

            spiderMock.Setup(service => service.GetLinkedPages(It.Is<Page>(page => page.Url == startUrl), It.IsAny<Uri>(), token))
                .ReturnsAsync(linksFoundByParser.Select(link => new Page(link, startUrl)));

            foreach (var link in linksFoundByParser)
            {
                spiderMock.Setup(service => service.GetLinkedPages(It.Is<Page>(page => page.Url == link), It.IsAny<Uri>(), token))
                    .ReturnsAsync(new List<Page>());
            }

            // Act 
            var manager = new SpiderManager(new NullLogger<SpiderService>(), spiderMock.Object);
            var result = await manager.CreateSiteMap(startUrl, token);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count());

            Assert.IsEmpty(result.First(dto => dto.Url == startUrl).ParentPagesUrl);

            foreach (var link in linksFoundByParser)
            {
                Assert.That(result.First(dto => dto.Url == link).ParentPagesUrl.Single().Key == startUrl);
            }
        }
    }
}