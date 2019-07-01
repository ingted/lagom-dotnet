using System;

public static class InstanceFactory
{
    public static Func<TE> CreateInstance<T, TE>()
    {
        return new Func<TE>(() => (TE)Activator.CreateInstance(typeof(T), null));
    }
}
