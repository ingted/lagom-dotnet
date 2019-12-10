// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


﻿using System;
using System.Threading.Tasks;
using Akka;

namespace wyvern.api.tests.ioc
{
    public class TestServiceImpl : TestService
    {
        public override Func<ServiceCall<User, string>> CreateUser
            => () => new ServiceCall<User, string>(user => Task.FromResult("1"));
        public override Func<string, ServiceCall<NotUsed, UserWithId>> GetUser
            => id => new ServiceCall<NotUsed, UserWithId>(_ => Task.FromResult(new UserWithId { Id = "1", Name = "some_name" }));
        public override Func<string, ServiceCall<NotUsed, Done>> DeleteUser
            => id => new ServiceCall<NotUsed, Done>(_ => Task.FromResult(Done.Instance));
        public override Func<string, ServiceCall<NotUsed, NotUsed>> HeadUser
            => id => new ServiceCall<NotUsed, NotUsed>(_ => Task.FromResult(NotUsed.Instance));
        public override Func<string, ServiceCall<NotUsed, NotUsed>> OptionsUser
            => id => new ServiceCall<NotUsed, NotUsed>(_ => Task.FromResult(NotUsed.Instance));
        public override Func<string, ServiceCall<User, Done>> PatchUser
            => id => new ServiceCall<User, Done>(user => Task.FromResult(Done.Instance));
        public override Func<string, ServiceCall<User, Done>> PutUser
            => id => new ServiceCall<User, Done>(user => Task.FromResult(Done.Instance));
        public override Func<string, ServiceCall<UserFriend, string>> CreateUserFriend
            => id => new ServiceCall<UserFriend, string>(friend => Task.FromResult("1.1"));
        public override Func<string, string, ServiceCall<NotUsed, UserFriend>> GetUserFriend
            => (id, id2) => new ServiceCall<NotUsed, UserFriend>(_ => Task.FromResult(new UserFriend { Id = "1.1", Name = "some_friend_name" }));
        public override Func<string, int, ServiceCall<NotUsed, UserFriend>> GetUserFriendByInt
            => (id, id2) => new ServiceCall<NotUsed, UserFriend>(_ => Task.FromResult(new UserFriend { Id = "1.1", Name = "some_friend_name" }));
    }
}