using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using System.Threading.Tasks;

namespace arachni.Helper.Tests
{
    [TestFixture]
    public class DocumentParserTests
    {
        [Test]
        public async Task GetLinksAsyncTest()
        {
            // Arrange
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"github-header.html");
            var document = await File.ReadAllTextAsync(path);

            var testTarget = new DocumentParser();

            // Act
            var links = await testTarget.GetLinksAsync(document);

            // Assert
            Assert.AreEqual(20, links.Count());
        }

        [Test]
        public async Task GetOnlyHttpOrHttpsLinksAsyncTest()
        {
            // Arrange
            var document = "<html>" +
                           "<a href='http://www.somedomain.com' />" +
                           "<a href='ftp://www.somedomain.com' />" +
                           "<a href='http://www.somedomain.com/a' />" +
                           "<a href='mailto:someone@example.com?Subject=Hello%20again'>Send mail!</a>" +
                           "<a href='http://www.somedomain.com/b' />" +
                           "</html>";

            var testTarget = new DocumentParser();

            // Act
            var links = await testTarget.GetLinksAsync(document);

            // Assert
            Assert.AreEqual(3, links.Count());
        }

        [Test]
        public async Task GetOnlyUniqueLinksAsyncTest()
        {   
            // Arrange
            var document = "<html>" +
                           "<a href='http://www.somedomain.com' />" +
                           "<a href='http://www.somedomain.com' />" +
                           "<a href='http://www.somedomain.com' />" +
                           "</html>";

            var testTarget = new DocumentParser();
            
            // Act
            var links = await testTarget.GetLinksAsync(document);
            
            // Assert
            Assert.AreEqual(1, links.Count());
        }
    }
}