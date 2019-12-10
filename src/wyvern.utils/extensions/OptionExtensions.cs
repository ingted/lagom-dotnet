// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using Akka.Util;

namespace wyvern.utils.extensions
{
    /// <summary>
    /// Extensions for Akka.Streams.Util.Option
    /// </summary>
    public static class OptionExtensions
    {
        public static bool ForAll<T>(this Option<T> o, Func<T, bool> f)
        {
            return !o.HasValue || f(o.Value);
        }

        /// <summary>
        /// Iterate on the option
        /// </summary>
        /// <param name="options"></param>
        /// <param name="func"></param>
        /// <typeparam name="T"></typeparam>
        public static void ForEach<T>(this Option<T> options, Action<T> func)
        {
            if (options.HasValue) func(options.Value);
        }

        /// <summary>
        /// Choose either the option value (if it has one) or the provided alternate
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T OrElse<T>(this Option<T> o, T e)
        {
            return o.HasValue ? o.Value : e;
        }

        /// <summary>
        /// Choose either the option value (if it has one) or the provided alternate
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T OrElseThrow<T>(this Option<T> o, Exception e)
        {
            if (o.HasValue) return o.Value;
            throw e;
        }

        /// <summary>
        /// Map the input type to the output type via the given delegate
        /// </summary>
        /// <param name="o"></param>
        /// <param name="f"></param>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static Option<E> Map<T, E>(
            this Option<T> o,
            Func<T, E> f)
        {
            return o.HasValue ? new Option<E>(f(o.Value)) : Option<E>.None;
        }
    }

    /// <summary>
    /// Option intializers
    /// </summary>
    public static class OptionInitializers
    {
        /// <summary>
        /// Create 'some' instance of T
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Option<T> Some<T>(T obj)
        {
            return new Option<T>(obj);
        }
    }
}
