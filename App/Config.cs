using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.KeywordsSearchService;

namespace App
{
    public class Config : IKeywordsSearchConfig
    {
        public int StackExchangeMaxConnections { get; set; }
        public int StackExchangeQueueMaxSize { get; set; }

        public void Validate()
        {
            ((IKeywordsSearchConfig)this).ValidateKeywordsSearchConfig();
        }
    }
}
