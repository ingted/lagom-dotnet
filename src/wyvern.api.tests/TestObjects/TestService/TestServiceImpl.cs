using System;
using System.Threading.Tasks;
using Akka;

namespace wyvern.api.tests.ioc
{
    public class TestServiceImpl : TestService
    {
        public override Func<Func<User, Task<string>>> CreateUser 
            => () => user => Task.FromResult("1");
        public override Func<string, Func<NotUsed, Task<UserWithId>>> GetUser
            => id => _ => Task.FromResult(new UserWithId { Id = "1", Name = "some_name" });
        public override Func<string, Func<NotUsed, Task<Done>>> DeleteUser
            => id => _ => Task.FromResult(Done.Instance);
        public override Func<string, Func<NotUsed, Task<NotUsed>>> HeadUser
            => id => _ => Task.FromResult(NotUsed.Instance);
        public override Func<string, Func<NotUsed, Task<NotUsed>>> OptionsUser
            => id => _ => Task.FromResult(NotUsed.Instance);
        public override Func<string, Func<User, Task<Done>>> PatchUser
            => id => user => Task.FromResult(Done.Instance);
        public override Func<string, Func<User, Task<Done>>> PutUser
            => id => user => Task.FromResult(Done.Instance);
        public override Func<string, Func<UserFriend, Task<string>>> CreateUserFriend
            => id => friend => Task.FromResult("1.1");
        public override Func<string, string, Func<NotUsed, Task<UserFriend>>> GetUserFriend
            => (id, id2) => _ => Task.FromResult(new UserFriend { Id = "1.1", Name = "some_friend_name" });
        public override Func<string, int, Func<NotUsed, Task<UserFriend>>> GetUserFriendByInt
            => (id, id2) => _ => Task.FromResult(new UserFriend { Id = "1.1", Name = "some_friend_name" });
    }
}