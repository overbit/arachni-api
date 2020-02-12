using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace arachni.Helper
{
    public static class UrlFormatHelper
    {
        public static bool IsAbsoluteUrl(this string url)
        {
            return Regex.IsMatch(url, @"http(s?)+:(\/?\/?)[^\s]+");
        }

        public static bool IsRelativeUrl(this string url)
        {
            return Regex.IsMatch(url, @"^([^\/]+\/[^\/].*$|^\/[^\/].*|\w+\.html)$");
        }

        public static string GetAbsoluteUrlString(Uri baseUri, string url)
        {
            var uri = new Uri(url, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(baseUri, uri);
            return uri.ToString();
        }
        
        public static bool IsMediaLink(string url)
        {
            if (url == null)
                return false;

            string[] stringArray = { ".png", ".jpg", ".jpeg", ".js", ".css" };
            return stringArray.Any(s => url.Contains(s));
        }
    }
}
