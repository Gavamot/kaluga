using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using App;
using App.KeywordsSearchService;
using App.KeywordsSearchService.Dto;
using FakeItEasy;
using NUnit.Framework;

namespace KalugaTest.KeywordsSearchService
{
    [TestFixture]
    class ParallelQueueKeywordsSearchServiceTest
    {
        class FakeStackExchangeHttpClient : IStackExchangeHttpClient
        {
            private Func<Task<IReadOnlyList<Item>>> generator;

            public FakeStackExchangeHttpClient(Func<Task<IReadOnlyList<Item>>> generator)
            {
                this.generator = generator;
            }

            public async Task<IReadOnlyList<Item>> GetItemsAsync(Keyword keyword, CancellationToken cansellationToken)
            {
                return await generator();
            }

            public static IReadOnlyList<Item> items = new List<Item>()
            {
                new Item(),
                new Item()
            };

            public static FakeStackExchangeHttpClient CreateBase()
            {
                return new FakeStackExchangeHttpClient(() => Task.FromResult(items));
            }
        }

        private ParallelQueueKeywordsSearchService service;
        private IStackExchangeHttpClient stackExchange;

        private const int queueMaxSize2 = 2;
        private IKeywordsSearchConfig config2 = CreateConfig(queueMaxSize2);

        private static IKeywordsSearchConfig CreateConfig(int queueMaxSize) =>
            new Config()
            {
                StackExchangeQueueMaxSize = queueMaxSize
            };
        
        [SetUp]
        public void Init()
        {
            stackExchange = A.Fake<IStackExchangeHttpClient>();
            service = new ParallelQueueKeywordsSearchService(stackExchange, config2);
        }

        [Test]
        public void GetItems_SimpleCase_Execution()
        {
            config2 = CreateConfig(queueMaxSize2);

            service = new ParallelQueueKeywordsSearchService(FakeStackExchangeHttpClient.CreateBase(), config2);

            var words = new[] { "1", "2", "3", "4", "5" };
            var res = service.GetItems(words);

            Assert.AreEqual(words.Length * 2, res.Count);
        }

        [Test]
        public void GetItems_Cancelation_CancelAllRequestAfterError()
        {
            var item = new Item();
 
            service = new ParallelQueueKeywordsSearchService(stackExchange, config2);

            A.CallTo(() => stackExchange.GetItemsAsync(A<Keyword>.Ignored, A<CancellationToken>.Ignored))
                .Throws<Exception>();

            var words = new[] { "1", "2", "3", "4", "5" };

            Assert.Throws<KeywordsSearchExecutionException>(()=> {
                var res = service.GetItems(words);
            });

            A.CallTo(() => stackExchange.GetItemsAsync(A<Keyword>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedTwiceOrLess();
        }

        [Test]
        public void GetItems_TwoWords_CheckQueueMaxSize()
        {
            var item = new Item();
            const int operationDuraionMs = 250;

            config2 = CreateConfig(queueMaxSize2);

            stackExchange = new FakeStackExchangeHttpClient(async () => {
                await Task.Delay(operationDuraionMs);
                return new List<Item>() { item };
            });

            service = new ParallelQueueKeywordsSearchService(stackExchange, config2);
            var words = new[] { "1", "2", "3", "4", "5" };
            var timer = Stopwatch.StartNew();
            var res = service.GetItems(words);
            timer.Stop();

            var actual = Math.Ceiling(((double)words.Length) / queueMaxSize2) * operationDuraionMs;

            Assert.IsTrue(timer.ElapsedMilliseconds >= actual);
            Assert.IsTrue(timer.ElapsedMilliseconds < words.Length * operationDuraionMs);
        }

        [Test]
        public void GetItems_OneWord_ReturnCorrect()
        {
            string parameter = "a";
            var item = new Item();

            A.CallTo(() => stackExchange.GetItemsAsync(A<Keyword>.Ignored, A<CancellationToken>.Ignored))
                .WithAnyArguments()
                .Returns(Task.FromResult((IReadOnlyList<Item>)new List<Item>() { item }));

            var res = service.GetItems(parameter);

            CollectionAssert.AreEquivalent(res, new List<Item>() { item });
        }

        [Test]
        public void GetItems_OneWord_OneRequest()
        {
            string parameter = "a";
            var item = new Item();
            A.CallTo(() => stackExchange.GetItemsAsync(A<Keyword>.Ignored, A<CancellationToken>.Ignored))
                .Returns(Task.FromResult((IReadOnlyList<Item>)new List<Item>() { item }));
            
            var res = service.GetItems(parameter);

            A.CallTo(() => stackExchange.GetItemsAsync(A<Keyword>.Ignored, A<CancellationToken>.Ignored))
                .MustHaveHappenedOnceExactly();
        }

        [Test]
        public void GetItems_PassBadArgumentsLongParameterInArray_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new[] { new string('q', 100) });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new[] { new string('q', 200) });
            });
        }

        [Test]
        public void GetItems_PassBadArgumentsEmptyParameterInArray_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new[] { "aaa", "" });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new[] { "aaa", null });
                service.GetItems(new[] { "", "aaa" });
                service.GetItems(new[] { null, "aaa" });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new[] { "", "aaa" });
            });

            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new[] { null, "aaa" });
            });
        }

        [Test]
        public void GetItems_PassBadArgumentsArrayNull_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(null);
            });
        }

        [Test]
        public void GetItems_PassBadArgumentsArrayEmpty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                service.GetItems(new string[0]);
            });
        }

        // TODO : Add tests for sorted result by created date DESC
        // TODO : Add tests for words to lower case
        // TODO : Add tests for distinct words 
        // TODO : Add tests for limited ParallelQueueKeywordsSearch request executes 

    }
}
