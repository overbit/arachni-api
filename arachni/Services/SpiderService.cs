using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using arachni.Helper;
using arachni.Models;
using Microsoft.Extensions.Logging;

namespace arachni.Services
{
    public class SpiderService : ISpiderService
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<SpiderService> logger;
        private readonly IDocumentParser documentParser;

        public SpiderService(HttpClient httpClient, ILogger<SpiderService> logger, IDocumentParser documentParser)
        {
            this.httpClient = httpClient;
            this.logger = logger;
            this.documentParser = documentParser;

        }

        public async Task<IEnumerable<Page>> GetLinkedPages(Page currentPage, Uri domain, CancellationToken token)
        {
            var pages = new List<Page>();
            
            try
            {
                var response = await httpClient.GetAsync(currentPage.Url, token);
                if (!response.IsSuccessStatusCode)
                    return pages;

                var htmlDocument = await response.Content.ReadAsStringAsync();
                var links = await documentParser.GetLinksAsync(htmlDocument);

                foreach (var link in links)
                {
                    logger.LogTrace($"Adding link to list to crawl : {link}");

                    try
                    {
                        var absUrl = UrlFormatHelper.GetAbsoluteUrlString(domain, link);

                        // Check for same sub-domain 
                        if (new Uri(absUrl).Host == domain.Host)
                        {
                            pages.Add(new Page(absUrl, currentPage.Url));
                        }
                    }
                    catch (UriFormatException e)
                    {
                        logger.LogError(e, $"Invalid url: {link}");
                    }
                }

                return pages;
            }
            catch (TaskCanceledException)
            {
                return pages;
            }

            
        }
    }
}
