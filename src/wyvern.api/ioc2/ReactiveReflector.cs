// ----------------------------------------------------------------------------
// Copyright (C) 2017-2019 Jonathan Nagy
// Copyright (C) 2016-2019 Lightbend Inc. <https://www.lightbend.com>
// ----------------------------------------------------------------------------


using System;
using System.Linq;
using wyvern.api.ioc;

namespace wyvern.api.ioc2
{
    public class ReactiveReflector
    {
        /// <summary>
        /// Get all entity types registered within the current app domain
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// ! WARN: this will pick up entities that are referenced by other service objects
        /// * TODO: need to constrain the lookup of these objects to just the ones referenced
        /// * by the loaded internal services
        /// </remarks>
        public static Type[] GetEntityTypes()
        {
            var entityType = typeof(ShardedEntity);
            var entities = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => entityType.IsAssignableFrom(x))
                .Where(x => x != entityType)
                .Where(x => !x.IsAbstract);
            return entities.ToArray();
        }

        /// <summary>
        /// Gets services implied to be remote by filtering out the
        /// ones that have a local implementation
        /// </summary>
        /// <returns></returns>
        public static Type[] GetRemoteServiceTypes()
        {
            var localBaseTypes = GetLocalServiceTypes()
                .Select(x => x.BaseType);
            var serviceType = typeof(Service);
            var services = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => serviceType.IsAssignableFrom(x))
                .Where(x => x != serviceType)
                .Where(x => x.IsAbstract);
            return services.Where(x => !localBaseTypes.Contains(x)).ToArray();

        }

        /// <summary>
        /// Gets the service types implied to be local which have a local
        /// internal implementation
        /// </summary>
        /// <returns></returns>
        public static Type[] GetLocalServiceTypes()
        {
            var serviceType = typeof(Service);
            var services = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => serviceType.IsAssignableFrom(x))
                .Where(x => x != serviceType)
                .Where(x => !x.IsAbstract);
            return services.ToArray();
        }
    }
}
