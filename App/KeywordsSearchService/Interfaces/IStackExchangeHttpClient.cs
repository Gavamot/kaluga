using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using App.KeywordsSearchService.Dto;

namespace App.KeywordsSearchService
{
    public interface IStackExchangeHttpClient
    {
        Task<IReadOnlyList<Item>> GetItemsAsync(Keyword keyword, CancellationToken cansellationToken);
    }
}
