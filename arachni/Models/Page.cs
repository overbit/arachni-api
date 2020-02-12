namespace arachni.Models
{
    public class Page
    {
        public Page(string url, string parentUrl = null)
        {
            Url = url;
            ParentUrl = parentUrl;
        }

        public string Url { get; set; }
        public string ParentUrl { get; set; }
    }
}
