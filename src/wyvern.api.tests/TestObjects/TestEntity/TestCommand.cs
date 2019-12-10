// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using wyvern.api.abstractions;
using wyvern.entity.command;

namespace wyvern.api.tests.TestObjects.TestEntity
{
    public abstract class TestCommand : AbstractCommand
    {
        public sealed class Get : TestCommand, IReplyType<TestState>
        {

        }

        public sealed class ChangeMode : TestCommand, IReplyType<TestEvent.ChangedMode>
        {
            public TestEntity.Mode NextMode { get; }
            public ChangeMode(TestEntity.Mode nextMode) => NextMode = nextMode;
        }

        public sealed class Add : TestCommand, IReplyType<TestEvent.Added>
        {
            public string Text { get; }
            public int Count { get; }

            public Add(string text, int count = 1)
            {
                Text = text;
                Count = count;
            }
        }
    }
}