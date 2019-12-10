// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;

namespace wyvern.utils
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Property)]
    public class ImmutableAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ImmutablePropertyAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method)]
    public class NoSideEffectsAttribute : Attribute
    {

    }
}
