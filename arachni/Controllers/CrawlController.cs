using System;
using System.Threading;
using System.Threading.Tasks;
using arachni.Configurations;
using arachni.Helper;
using arachni.Managers;
using arachni.Models.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace arachni.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CrawlController : ControllerBase
    {
        private readonly ILogger<CrawlController> logger;
        private readonly ISpiderManager manager;

        public CrawlController(ILogger<CrawlController> logger, ISpiderManager manager)
        {
            this.logger = logger;
            this.manager = manager;
        }

        /// <summary>
        /// Generate the sitemap starting from the provided url
        /// </summary>
        /// <remarks>
        /// Sample
        /// </remarks>
        /// <param name="url">Url that will be used as staring point for crawl. The crawler will only follow links from the same sub-domain</param>
        /// <param name="timeout">Timeout in seconds. The crawler will stop if the timeout is reached. Max timeout is 360 seconds</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SiteMap>> Post([FromQuery]string url, int timeout = 0)
        {
            if (string.IsNullOrWhiteSpace(url) ||
                !url.IsAbsoluteUrl())
                return BadRequest();

            timeout = Math.Min(timeout, KestrelConfiguration.KestrelDefaultTimeoutInSeconds);

            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));

            var pages = manager.CreateSiteMap(url, cts.Token);
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);

            return Ok(new SiteMap
            {
                Pages = await pages,
                RequestedDomain = $"{uri.Scheme}://{uri.Host}",
                StartUrl = url,
                Completed = !cts.IsCancellationRequested
            });
        }
    }
}
