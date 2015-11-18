using System;
using Common.Testing.NUnit;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using StackExchange.Redis;

namespace WebAPI.OutputCache.Redis.Tests.Methods
{
    [TestFixture]
    public class ContainsTests : TestBase
    {
        protected RedisOutputCache RedisApiOutputCache;
        IRedisConnectionSettings _connectionSettings;
        ConnectionMultiplexer _multiplexer;
        UserFixture _fixture;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _connectionSettings = new RedisConnectionSettings { ConnectionString = "localhost:6379", Db = 2 };
            _multiplexer = ConnectionMultiplexer.Connect(_connectionSettings.ConnectionString);
        }

        protected override void FinalizeSetUp()
        {
            _fixture = FixtureRepository.Create<UserFixture>();

            RedisApiOutputCache = new RedisOutputCache(new JsonSerializer(), _connectionSettings);

            RedisApiOutputCache.Add(_fixture.Id.ToString(), _fixture, DateTime.Now.AddSeconds(60));

        }

        private void ClearDb()
        {
            _multiplexer.GetServer(_connectionSettings.ConnectionString).FlushDatabase(_connectionSettings.Db);
        }


        [Test]
        public void removes_item_from_cache()
        {
            bool result = RedisApiOutputCache.Contains(_fixture.Id.ToString());

            result.Should().BeTrue();
        }

        [Test]
        public void returns_false_if_item_is_not_in_collection()
        {
            var result = RedisApiOutputCache.Contains("I_AM_A_BAD_KEY");

            result.Should().BeFalse();
        }

    }
}