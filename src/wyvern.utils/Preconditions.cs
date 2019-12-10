// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;

namespace wyvern.utils
{
    /// <summary>
    /// Set of exception throwing assertions
    /// </summary>
    public static class Preconditions
    {
        /// <summary>
        /// Asserts that the given entity is not null or throws a null reference exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void IsNotNull<T>(this T thing, string errorMessage) where T : class
        {
            if (thing == null) throw new NullReferenceException(errorMessage);
        }
    }

}
