using System;

namespace WebAPI.OutputCache.Redis.Tests
{
    public class UserFixture
    {
        public UserFixture()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}