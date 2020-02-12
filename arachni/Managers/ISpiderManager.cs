using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using arachni.Models;
using arachni.Models.Dtos;

namespace arachni.Managers
{
    public interface ISpiderManager
    {
        Task<IEnumerable<PageDto>> CreateSiteMap(string baseUrl, CancellationToken token);
    }
}
