using System;
using System.Collections.Generic;
using Commonality;

namespace GenController.Portable.Tests.Mocks
{
    public class MockSettings : ISettings
    {
        Dictionary<string, object> Storage = new Dictionary<string, object>();

        public IEnumerable<string> GetCompositeKey(string key)
        {
            if (Storage.ContainsKey(key))
            {
                return (IEnumerable<string>)Storage[key];
            }
            else
                return null;
        }

        public string GetKey(string key)
        {
            if (Storage.ContainsKey(key))
            {
                return (string)Storage[key];
            }
            else
                return null;
        }

        public string GetKeyValueWithDefault(string key, string defaultvalue)
        {
            return GetKey(key) ?? defaultvalue;
        }

        public void SetCompositeKey(string key, IEnumerable<string> values)
        {
            Storage[key] = values;
        }

        public void SetKey(string key, string value)
        {
            Storage[key] = value;
        }
    }
}
