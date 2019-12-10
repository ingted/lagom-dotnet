// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------

using System;
using wyvern.api.ioc;
using wyvern.entity.command;
using wyvern.entity.@event;
using wyvern.entity.state;

namespace wyvern.api.ioc2
{
    public delegate T EntityFactory<T, TC, TE, TS>()
        where T : ShardedEntity<TC, TE, TS>
        where TC : AbstractCommand
        where TE : AbstractEvent
        where TS : AbstractState;

    public static class InstanceFactory
    {
        /// <summary>
        /// Generic factory used to reflectively create an Entity typed instance of the type 'T'
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TE"></typeparam>
        /// <returns></returns>
        public static Func<TE> CreateInstance<T, TE>()
        {
            return new Func<TE>(() => (TE)Activator.CreateInstance(typeof(T), null));
        }
    }
}