using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using arachni.Models;

namespace arachni.Services
{
    public interface ISpiderService
    {
        Task<IEnumerable<Page>> GetLinkedPages(Page currentPage, Uri domain, CancellationToken token);
    }
}
