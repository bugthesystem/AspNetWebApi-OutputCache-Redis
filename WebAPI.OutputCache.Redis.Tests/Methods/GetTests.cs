using System;
using Common.Testing.NUnit;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using StackExchange.Redis;

namespace WebAPI.OutputCache.Redis.Tests.Methods
{
    [TestFixture]
    public class GetTests : TestBase
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
        public void retrieves_item_from_cache()
        {
            var result = RedisApiOutputCache.Get<UserFixture>(_fixture.Id.ToString());

            result.Should().NotBeNull();
            result.Id.ShouldBeEquivalentTo(_fixture.Id);
            result.Name.ShouldBeEquivalentTo(_fixture.Name);

            result.DateOfBirth.Day.ShouldBeEquivalentTo(_fixture.DateOfBirth.Day);
            result.DateOfBirth.Month.ShouldBeEquivalentTo(_fixture.DateOfBirth.Month);
            result.DateOfBirth.Year.ShouldBeEquivalentTo(_fixture.DateOfBirth.Year);

        }
    }
}