using System;
using Common.Testing.NUnit;
using FluentAssertions;
using NUnit.Framework;
using Ploeh.AutoFixture;
using StackExchange.Redis;

namespace WebAPI.OutputCache.Redis.Tests.Methods
{
    [TestFixture]
    public class AddTests : TestBase
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
        public void adds_item_to_cache()
        {
            //ClearDb();
            var fixture = FixtureRepository.Create<UserFixture>();
            string key = fixture.Id.ToString();

            RedisApiOutputCache.Add(key, fixture, DateTime.Now.AddSeconds(60));
            var result = RedisApiOutputCache.Get<UserFixture>(key);

            result.Should().NotBeNull();
            result.Id.ShouldBeEquivalentTo(fixture.Id);
            result.Name.ShouldBeEquivalentTo(fixture.Name);


        }

        [Test]
        public void added_item_stored_with_expiry()
        {
            var fixture = FixtureRepository.Create<UserFixture>();

            var key = fixture.Id.ToString();

            var expiration = DateTime.UtcNow.AddSeconds(60);

            fixture.ExpireAt = expiration;

            RedisApiOutputCache.Add(fixture.Id.ToString(), fixture, expiration);

            var result = RedisApiOutputCache.Get<UserFixture>(key);

            //todo: would be good to check they are the same value.. without this rubbish!
            expiration.Day.ShouldBeEquivalentTo(result.ExpireAt.Day);
            expiration.Month.ShouldBeEquivalentTo(result.ExpireAt.Month);
            expiration.Year.ShouldBeEquivalentTo(result.ExpireAt.Year);
            expiration.Hour.ShouldBeEquivalentTo(result.ExpireAt.Hour);
            expiration.Minute.ShouldBeEquivalentTo(result.ExpireAt.Minute);
            expiration.Second.ShouldBeEquivalentTo(result.ExpireAt.Second);
            expiration.Millisecond.ShouldBeEquivalentTo(result.ExpireAt.Millisecond);
        }

        [Test]
        public void adding_item_with_duplicate_key_updates_original()
        {
            var fixture = FixtureRepository.Create<UserFixture>();

            var key = "user";

            RedisApiOutputCache.Add(key, fixture, DateTime.Now.AddSeconds(60));

            var differentUser = FixtureRepository.Create<UserFixture>();

            RedisApiOutputCache.Add(key, differentUser, DateTime.Now.AddSeconds(60));

            var result = RedisApiOutputCache.Get<UserFixture>(key);

            result.Should().NotBeNull();
            result.Id.ShouldBeEquivalentTo(differentUser.Id);
            result.Name.ShouldBeEquivalentTo(differentUser.Name);
        }
    }
}