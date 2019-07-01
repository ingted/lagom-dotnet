using System;
using System.Linq;
using wyvern.api;
using wyvern.api.ioc;

public class ReactiveReflector
{
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
