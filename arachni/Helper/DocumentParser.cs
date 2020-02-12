using System.Collections.Generic;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace arachni.Helper
{
    public class DocumentParser : IDocumentParser
    {
        public DocumentParser()
        {
        }

        public async Task<IEnumerable<string>> GetLinksAsync(string html)
        {
            return await Task.Run(() => GetUniqueLinks(html));
        }

        IEnumerable<string> GetUniqueLinks(string html)
        {
            var links = new Dictionary<string, int>();

            if (string.IsNullOrWhiteSpace(html))
            {
                return links.Keys;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var nodes = doc.DocumentNode.SelectNodes("//a[@href]");

            if (nodes != null)
            {
                foreach (var n in nodes)
                {
                    string href = n.Attributes["href"].Value;

                    if (href.IsRelativeUrl() ||
                        href.StartsWith("http") && !UrlFormatHelper.IsMediaLink(href))
                    {
                        links.TryAdd(href, 1);
                    }
                }
            }
            return links.Keys;
        }
    }
}
