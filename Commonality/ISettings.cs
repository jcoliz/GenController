using System.Collections.Generic;

namespace Commonality
{
    /// <summary>
    /// Defines a platform-dependent service to retrieve settings
    /// </summary>
    public interface ISettings
    {
        string GetKey(string key); // Or null if not found

        void SetKey(string key, string value);

        IEnumerable<string> GetCompositeKey(string key); // Or empty if not found

        void SetCompositeKey(string key, IEnumerable<string> values);
    }
}
