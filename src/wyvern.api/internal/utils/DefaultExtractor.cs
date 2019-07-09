using System.Collections.Generic;
using wyvern.api.abstractions;

namespace wyvern.utils
{
    /// <summary>
    /// Extracts properties of a message into the given dictionary
    /// </summary>
    /// <remarks>
    /// Note: by default this does nothing
    /// </remarks>
    public class DefaultExtractor : IMessagePropertyExtractor
    {
        public Dictionary<string, object> Extract<T>(T obj)
        {
            return new Dictionary<string, object>();
        }
    }

}