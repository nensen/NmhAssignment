using NmhAssignment.Services;
using System.Collections.Concurrent;

namespace KeyValueWebApi.Services
{
    /// <summary>
    /// Cache wrapper
    /// </summary>
    public class KeyValueStorage
    {
        private readonly ConcurrentDictionary<int, KeyValueModel> storage = new();

        public ConcurrentDictionary<int, KeyValueModel> Storage => storage;
    }
}