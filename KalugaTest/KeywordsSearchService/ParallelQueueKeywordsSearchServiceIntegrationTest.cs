using System.Net;
using System.Net.Http;
using App;
using App.KeywordsSearchService;
using FakeItEasy;
using NUnit.Framework;

namespace Integration
{
    [TestFixture]
    class ParallelQueueKeywordsSearchServiceIntegrationTest
    {
        private ParallelQueueKeywordsSearchService service;
        private IStackExchangeHttpClient stackExchange;

        private const int queueMaxSize2 = 2;
        private IKeywordsSearchConfig config2 = CreateConfig(queueMaxSize2);
        private const int queueMaxSize6 = 6;
        private IKeywordsSearchConfig config6 = CreateConfig(queueMaxSize6);

        private static IKeywordsSearchConfig CreateConfig(int queueMaxSize) =>
            new Config()
            {
                StackExchangeQueueMaxSize = queueMaxSize
            };

   
        private HttpClient client = new HttpClient(new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            MaxConnectionsPerServer = 6
        });

        [SetUp]
        public void Init()
        {
            var factory = A.Fake<IHttpClientFactory>();
            A.CallTo(() => factory.CreateClient(A<string>.Ignored))
                .WithAnyArguments()
                .Returns(client);
            stackExchange = new StackExchangeHttpClient(factory);
            service = new ParallelQueueKeywordsSearchService(stackExchange, config2);
        }

        [Test]
        public void GetItems_OneWord_ShoodReturn100Items()
        {
            var res = service.GetItems("linq");
            Assert.AreEqual(res.Count, 100);
        }

        [Test]
        public void GetItems_SixWordsOneReturn0_ShoodReturn500Items()
        {
            var res = service.GetItems("linq", "sql", "facade", "ttl", "java", "_0");
            Assert.AreEqual(res.Count, 500);
        }

        [Test]
        public void GetItems_FiveWords_ShoodReturn500Items()
        {
            var res = service.GetItems("linq", "sql", "facade", "ttl", "java", "_0");
            Assert.IsTrue(res.Count >= 500 && res.Count <= 600);
        }
    }
}
