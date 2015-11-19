using System;
using System.Threading;
using Common.Testing.NUnit;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using StackExchange.Redis;

namespace WebAPI.OutputCache.Redis.Tests.Methods
{
    [TestFixture]
    public class GetGenericTests : TestBase
    {
        protected RedisOutputCache RedisApiOutputCache;
        IRedisConnectionSettings _connectionSettings;
        ConnectionMultiplexer _multiplexer;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            _connectionSettings = new RedisConnectionSettings { ConnectionString = "localhost:6379", Db = 2 };
            _multiplexer = ConnectionMultiplexer.Connect(_connectionSettings.ConnectionString);
        }

        protected override void FinalizeSetUp()
        {
            RedisApiOutputCache = new RedisOutputCache(new JsonSerializer(), _connectionSettings);
        }

        private void ClearDb()
        {
            _multiplexer.GetServer(_connectionSettings.ConnectionString).FlushDatabase(_connectionSettings.Db);
        }


        [Test]
        public void retrieves_item_from_cache()
        {
            var fixture = FixtureRepository.Create<UserFixture>();
            RedisApiOutputCache.Add(fixture.Id.ToString(), fixture, DateTime.Now.AddSeconds(60));
            var result = RedisApiOutputCache.Get<UserFixture>(fixture.Id.ToString());

            result.Should().NotBeNull();
            result.Should().BeOfType<UserFixture>();

            result.Id.ShouldBeEquivalentTo(fixture.Id);
            result.Name.ShouldBeEquivalentTo(fixture.Name);

            result.DateOfBirth.Day.ShouldBeEquivalentTo(fixture.DateOfBirth.Day);
            result.DateOfBirth.Month.ShouldBeEquivalentTo(fixture.DateOfBirth.Month);
            result.DateOfBirth.Year.ShouldBeEquivalentTo(fixture.DateOfBirth.Year);

        }

        [Test]
        public void returns_null_if_item_not_in_collection()
        {
            var result = RedisApiOutputCache.Get<UserFixture>("unknown key");

            result.Should().BeNull();
        }

        [Test]
        public void does_not_return_item_that_has_expired()
        {
            //TODO: FIX ME
            var fixture = FixtureRepository.Create<UserFixture>();
            RedisApiOutputCache.Add("expired-item", fixture, DateTime.Now.AddSeconds(3));

            Thread.Sleep(TimeSpan.FromSeconds(5));

            var result = RedisApiOutputCache.Get<UserFixture>("expired-item");

            result.Should().BeNull();
        }
    }
}