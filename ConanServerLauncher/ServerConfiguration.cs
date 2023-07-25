using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConanServerLauncher
{
    internal class ServerConfiguration
    {
        public string Name { get; set; }
        public string LaunchOptions { get; set; }
        public string RestartTime { get; set; }
        public ProcessPriorityClass ProcessPriority { get; set; }
    }
}
