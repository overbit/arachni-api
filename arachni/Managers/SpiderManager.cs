using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using arachni.Models;
using arachni.Models.Dtos;
using arachni.Services;
using Microsoft.Extensions.Logging;

namespace arachni.Managers
{
    public class SpiderManager : ISpiderManager
    {
        private const int MaxTasks = 8;
        private readonly ILogger<SpiderService> logger;
        private readonly ISpiderService spider;

        private Uri targetedDomain { get; set; }
        private ConcurrentDictionary<string, PageDto> pagesCrawled { get; } 
        private ConcurrentQueue<Page> pagesToCrawl { get; }
        
        public SpiderManager(ILogger<SpiderService> logger, ISpiderService spider)
        {
            this.logger = logger;
            this.spider = spider;

            pagesCrawled = new ConcurrentDictionary<string, PageDto>();
            pagesToCrawl = new ConcurrentQueue<Page>();
        }

        public async Task<IEnumerable<PageDto>> CreateSiteMap(string baseUrl, CancellationToken token)
        {
            targetedDomain = new Uri(baseUrl);

            pagesToCrawl.Enqueue(new Page(baseUrl));

            do
            {
                var tasks = new List<Task>();
                for (int i = 0; i < MaxTasks; i++)
                {
                    tasks.Add(CrawlNext(token));
                }

                await Task.WhenAll(tasks.ToArray());

                logger.LogInformation($"Visited/ToVisit: {pagesCrawled.Count}/{pagesToCrawl.Count}");
            }
            while (pagesToCrawl.Count > 0 && !token.IsCancellationRequested);

            logger.LogInformation(token.IsCancellationRequested
                ? "Scan incomplete, timeout reached"
                : "Scan completed");

            return pagesCrawled.Values;
        }

        private async Task CrawlNext(CancellationToken token)
        {
            pagesToCrawl.TryDequeue(out var currentPage);

            if (currentPage == null)
            {
                return;
            }

            if (pagesCrawled.ContainsKey(currentPage.Url))
            {
                if (currentPage.ParentUrl != null) 
                    pagesCrawled[currentPage.Url].ParentPagesUrl.TryAdd(currentPage.ParentUrl, 0);
                
                return;
            }

            var pagesFound = await spider.GetLinkedPages(currentPage, targetedDomain, token);

            foreach (var page in pagesFound)
            {
                pagesToCrawl.Enqueue(page);
            }

            var dto = new PageDto(currentPage.Url, new ConcurrentDictionary<string, short>());

            if (currentPage.ParentUrl != null)
                dto.ParentPagesUrl.TryAdd(currentPage.ParentUrl, 0);

            pagesCrawled.TryAdd(dto.Url, dto);
        }
    }
}
