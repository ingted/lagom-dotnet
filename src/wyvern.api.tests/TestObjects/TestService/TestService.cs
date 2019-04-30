using System;
using System.Threading.Tasks;
using Akka;
using wyvern.api.abstractions;

namespace wyvern.api.tests.ioc
{
    public abstract class TestService : Service
    {
        public abstract Func<ServiceCall<User, string>> CreateUser { get; }
        public abstract Func<string, ServiceCall<NotUsed, UserWithId>> GetUser { get; }
        public abstract Func<string, ServiceCall<NotUsed, Done>> DeleteUser { get; }
        public abstract Func<string, ServiceCall<NotUsed, NotUsed>> HeadUser { get; }
        public abstract Func<string, ServiceCall<NotUsed, NotUsed>> OptionsUser { get; }
        public abstract Func<string, ServiceCall<User, Done>> PatchUser { get; }
        public abstract Func<string, ServiceCall<User, Done>> PutUser { get; }
        public abstract Func<string, ServiceCall<UserFriend, string>> CreateUserFriend { get; }
        public abstract Func<string, string, ServiceCall<NotUsed, UserFriend>> GetUserFriend { get; }
        public abstract Func<string, int, ServiceCall<NotUsed, UserFriend>> GetUserFriendByInt { get; }

        public override IDescriptor Descriptor => Named("test-service")
            .WithCalls(
                /* Test Each Method Type */
                RestCall(Method.POST, "/api/users", CreateUser),
                RestCall(Method.GET, "/api/user/{id}", GetUser),
                RestCall(Method.DELETE, "/api/user/{id}", DeleteUser),
                RestCall(Method.HEAD, "/api/user/{id}", HeadUser),
                RestCall(Method.OPTIONS, "/api/user/{id}", OptionsUser),
                RestCall(Method.PATCH, "/api/user/{id}", PatchUser),
                RestCall(Method.PUT, "/api/user/{id}", PutUser),
                /* Test Multiple Parameters */
                RestCall(Method.POST, "/api/user/{id}/friends", CreateUserFriend),
                RestCall(Method.GET, "/api/user/{id}/friend/{id2}", GetUserFriend),
                /* Test Route Constraints */
                RestCall(Method.GET, "/api/user/{id}/friend-seq/{id2:int}", GetUserFriendByInt)
            );

    }
}