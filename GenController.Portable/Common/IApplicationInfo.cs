using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// Provides information about the application to components who might like to discover it
    /// </summary>
    public interface IApplicationInfo
    {
        string Title { get; }
        string Version { get; }
    }
}
