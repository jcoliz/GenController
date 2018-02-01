using System.Collections.Generic;

namespace Commonality
{
    /// <summary>
    /// Defines a platform-dependent service to retrieve settings
    /// </summary>
    public interface ISettings
    {
        void SetKey(string key, string value);
        string GetKey(string key);
        string GetKeyValueWithDefault(string key, string defaultvalue);
        IEnumerable<string> GetCompositeKey(string key);
        void SetCompositeKey(string key, IEnumerable<string> values);
    }
}
