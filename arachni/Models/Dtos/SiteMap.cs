using System.Collections.Generic;

namespace arachni.Models.Dtos
{
    public class SiteMap
    {
        public string RequestedDomain { get; set; }

        public string StartUrl { get; set; }

        public bool Completed { get; set; }

        public IEnumerable<PageDto> Pages { get; set; }
    }
}
