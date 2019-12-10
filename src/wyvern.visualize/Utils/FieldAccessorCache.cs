// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


﻿using System;
using System.Collections.Concurrent;

namespace wyvern.visualize.Utils
{
    internal static class FieldAccessorCache
    {
        private static ConcurrentDictionary<string, FieldAccesor> _accessors = new ConcurrentDictionary<string, FieldAccesor>();

        public static FieldAccesor Get(Type type, string fieldName)
        {
            var fullName = $"{type.FullName}.{fieldName}";
            FieldAccesor result;
            if (!_accessors.TryGetValue(fullName, out result))
            {
                result = new FieldAccesor(type, fieldName);
                _accessors.TryAdd(fullName, result);
            }
            return result;
        }
    }
}
