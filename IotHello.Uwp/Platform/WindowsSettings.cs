using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace GenController.Uwp.Platform
{
    /// <summary>
    /// Windows platform-specific way to get settings
    /// </summary>
    public class WindowsSettings : ISettings
    {
        public IEnumerable<string> GetCompositeKey(string name)
        {
            List<string> result = new List<string>();

            var ApplicationSettings = ApplicationData.Current.LocalSettings.Values;
            if (ApplicationSettings.ContainsKey(name))
            {
                var composite = (ApplicationDataCompositeValue)ApplicationSettings[name];
                if (composite.ContainsKey("count"))
                {
                    int count = (int)composite["count"];
                    for (int i = 0; i < count; i++)
                    {
                        result.Add(composite[i.ToString()] as string);
                    }
                }
            }

            return result;
        }

        public string GetKey(string key)
        {
            var ApplicationSettings = ApplicationData.Current.LocalSettings.Values;

            string result = null;

            if (ApplicationSettings.Keys.Contains(key))
            {
                result = ApplicationSettings[key] as string;
            }

            return result;
        }

        public string GetKeyValueWithDefault(string key, string defaultvalue)
        {
            return GetKey(key) ?? defaultvalue;
        }

        public void SetCompositeKey(string name, IEnumerable<string> values)
        {
            ApplicationDataCompositeValue composite =
                new ApplicationDataCompositeValue();

            composite["count"] = values.Count();
            int i = 0;
            foreach (var value in values)
                composite[i++.ToString()] = value;

            var ApplicationSettings = ApplicationData.Current.LocalSettings.Values;

            if (!ApplicationSettings.Keys.Contains(name))
            {
                ApplicationSettings.Add(name, composite);
            }
            else
            {
                ApplicationSettings[name] = composite;
            }
        }

        public void SetKey(string name, string value)
        {
            var ApplicationSettings = ApplicationData.Current.LocalSettings.Values;

            if (!ApplicationSettings.Keys.Contains(name))
            {
                ApplicationSettings.Add(name, value);
            }
            else
            {
                ApplicationSettings[name] = value;
            }
        }
    }
}
