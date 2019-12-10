// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


namespace wyvern.api.hello.filters
{
    public class User
    {
        public string Name { get; }

        public User(string name)
        {
            Name = name;
        }
    }
}