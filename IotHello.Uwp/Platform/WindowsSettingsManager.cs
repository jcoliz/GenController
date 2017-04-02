using ManiaLabs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace IotHello.Uwp.Platform
{
    public class WindowsSettingsManager : IPlatformSettingsManager
    {
        public IEnumerable<string> GetCompositeKey(string name)
        {
            List<string> result = new List<string>();

            /*
            Windows.Storage.ApplicationDataCompositeValue composite = 
               (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["exampleCompositeSetting"];

            if (composite == null)
            {
               // No data
            }
            else
            {
               // Access data in composite["intVal"] and composite["strVal"]
            }
            */
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

        public string GetKeyValue(string name)
        {
            var ApplicationSettings = ApplicationData.Current.LocalSettings.Values;

            string result = null;

            if (ApplicationSettings.Keys.Contains(name))
            {
                result = ApplicationSettings[name] as string;
            }

            return result;
        }

        public void SetCompositeKey(string name, IEnumerable<string> values)
        {
            /*
            Windows.Storage.ApplicationDataCompositeValue composite = 
                new Windows.Storage.ApplicationDataCompositeValue();
            composite["intVal"] = 1;
            composite["strVal"] = "string";

            localSettings.Values["exampleCompositeSetting"] = composite;
            */
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
