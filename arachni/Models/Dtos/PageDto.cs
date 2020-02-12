using System.Collections.Concurrent;

namespace arachni.Models.Dtos
{
    public class PageDto
    {
        public PageDto(string url, ConcurrentDictionary<string, short> parentPagesUrl)
        {
            Url = url;
            ParentPagesUrl = parentPagesUrl;
        }

        public string Url { get; set; }

        public ConcurrentDictionary<string, short> ParentPagesUrl { get; set; }
    }
}
