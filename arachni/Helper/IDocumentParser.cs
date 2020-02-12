using System.Collections.Generic;
using System.Threading.Tasks;

namespace arachni.Helper
{
    public interface IDocumentParser
    {
        Task<IEnumerable<string>> GetLinksAsync(string html);
    }
}
