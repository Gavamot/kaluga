using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.KeywordsSearchService.Dto;
using System.Collections.Concurrent;
using System.Threading;

namespace App.KeywordsSearchService
{
    public class KeywordsSearchExecutionException : Exception
    {
        public KeywordsSearchExecutionException(string message, Exception e = null) : base(message, e)
        {

        }
    }

    public class ParallelQueueKeywordsSearchService : IKeywordsSearchService
    {
        private readonly IStackExchangeHttpClient stackExchange;
        private readonly IKeywordsSearchConfig config;

        public ParallelQueueKeywordsSearchService(IStackExchangeHttpClient stackExchange, IKeywordsSearchConfig config)
        {
            this.stackExchange = stackExchange;
            this.config = config;
        }

        public IReadOnlyList<Item> GetItems(IEnumerable<string> words)
        {
            var keywords = GetKeywords(words);
            var keywordsQueue = new ConcurrentQueue<Keyword>(keywords);

            int httpWorkersCount = Math.Min(config.StackExchangeQueueMaxSize, keywords.Length);
            var tasks = new Task<List<Item>>[httpWorkersCount];
            var cts = new CancellationTokenSource();

            for(int i = 0; i < httpWorkersCount; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    var res = new List<Item>();
                    while (!keywordsQueue.IsEmpty)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        if (keywordsQueue.TryDequeue(out var keyword))
                        {
                            var items = await stackExchange.GetItemsAsync(keyword, cts.Token);
                            res.AddRange(items);
                        }
                    }
                    return res;
                });
            }

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException e)
            {
                throw new KeywordsSearchExecutionException("Error ", e);
            }

            var res = ArgegateTasksResult(tasks);
            return res.OrderBy(x => x.CreationDate).ToArray();
        }

        public IReadOnlyList<Item> GetItems(params string[] words)
            => GetItems((IEnumerable<string>)words);
        
        private Keyword[] GetKeywords(IEnumerable<string> words)
        {
            if (words == null)
                throw new ArgumentException("parameter words[collection] is equal null");
            if (!words.Any())
                throw new ArgumentException("parameter words[collection] is empty");
            if (words.Any(x => string.IsNullOrWhiteSpace(x)))
                throw new ArgumentException("parameter words[collection] has null or empty element");

            return words.Select(x=> x.ToLower())
                .Distinct()
                .Select(x => new Keyword(x))
                .ToArray();
        }

        private List<Item> ArgegateTasksResult(Task<List<Item>>[] tasks)
        {
            var res = new List<Item>();
            foreach(var t in tasks)
            {
                var items = t.Result;
                res.AddRange(items);
            }
            return res;
        }
    }
}
