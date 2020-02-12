using System;
using arachni.Helper;
using NUnit.Framework;

namespace arachni.Helper.Tests
{
    [TestFixture()]
    public class UrlFormatHelperTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase("https://domain.com/wp", ExpectedResult = true)]
        [TestCase("http://www.domain.com/wp", ExpectedResult = true)]
        [TestCase("/food.html", ExpectedResult = false)]
        [TestCase("food.html", ExpectedResult = false)]
        public bool IsAbsoluteUrlTest(string url)
        {
            // Arrange
            // Act
            var result = url.IsAbsoluteUrl();
            // Assert
            return result;
        }

        [Test]
        [TestCase("https://domain.com/wp", ExpectedResult = false)]
        [TestCase("http://www.domain.com/wp", ExpectedResult = false)]
        [TestCase("/food.html", ExpectedResult = true)]
        [TestCase("food.html", ExpectedResult = true)]
        public bool IsRelativeUrlTest(string url)
        {
            // Arrange
            // Act
            var result = url.IsRelativeUrl();
            // Assert
            return result;
        }

        [Test]
        [TestCase("https://domain.com/wp", ExpectedResult = "https://domain.com/wp")]
        [TestCase("http://domain.com/wp", ExpectedResult = "http://domain.com/wp")]
        [TestCase("/food.html", ExpectedResult = "https://www.pizza.com/food.html")]
        [TestCase("food.html", ExpectedResult = "https://www.pizza.com/food.html")]
        public string GetAbsoluteUrlStringTest(string url)
        {
            // Arrange
            var baseUrl = new Uri("https://www.pizza.com");

            // Act
            var result = UrlFormatHelper.GetAbsoluteUrlString(baseUrl, url);

            // Assert
            return result;
        }

        [Test]
        [TestCase("https://domain.com/wp.js")]
        [TestCase("http://www.domain.com/wp.png")]
        [TestCase("http://www.domain.com/wp.min.js")]
        [TestCase("http://www.domain.com/food.css")]
        [TestCase("http://www.domain.com/food.css?v=123")]
        public void IsMediaLinkTest(string url)
        {
            // Arrange
            // Act
            var result = UrlFormatHelper.IsMediaLink(url);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        [TestCase("https://domain.com/wp.html")]
        [TestCase("http://www.domain.com/wp.htm")]
        public void IsNotMediaLinkTest(string url)
        {
            // Arrange
            // Act
            var result = UrlFormatHelper.IsMediaLink(url);

            // Assert
            Assert.IsFalse(result);
        }
    }
}