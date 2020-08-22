using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.KeywordsSearchService.Dto;
using Microsoft.AspNetCore.Http.Features;

namespace App.KeywordsSearchService
{
    public interface IKeywordsSearchService
    {
        IReadOnlyList<Item> GetItems(IEnumerable<string> words);
    }
}
