using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.KeywordsSearchService
{
    public class KeywordsSearchConfigException : Exception
    {
        public KeywordsSearchConfigException(string message) : base(message)
        {

        }
    }

    public interface IKeywordsSearchConfig
    {
        int StackExchangeMaxConnections { get; }
        int StackExchangeQueueMaxSize { get; }

        /// <exception cref="KeywordsSearchConfigException">Wrong settings</exception>
        public void ValidateKeywordsSearchConfig()
        {
            if (StackExchangeMaxConnections < 1 || StackExchangeMaxConnections > 50)
                throw new KeywordsSearchConfigException($"StackExchangeMaxConnections must be in interval 1 to 50");
            if (StackExchangeQueueMaxSize < 1 || StackExchangeQueueMaxSize > 100)
                throw new KeywordsSearchConfigException($"StackExchangeQueueMaxSize must be in interval 1 to 99");
        }
    }
}
