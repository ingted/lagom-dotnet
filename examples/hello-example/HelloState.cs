// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.entity.state;

public class HelloState : AbstractState
{
    const string DEFAULT_GREETING = "Hello";

    bool Created { get; } = false;

    public string Name { get; }
    public string Greeting { get; } = DEFAULT_GREETING;

    public HelloState() { }

    HelloState(string name, string greeting, bool created)
    {
        Name = name;
        Greeting = greeting;
        Created = created;
    }

    public HelloState WithCreated() => new HelloState(Name, Greeting, true);

    public HelloState WithName(string name) => new HelloState(name, Greeting, Created);

    public HelloState WithGreeting(string greeting) => new HelloState(Name, greeting, Created);

}