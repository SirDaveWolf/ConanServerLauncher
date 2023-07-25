using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConanServerLauncher
{
    internal class RunningProcessInformation
    {
        public Process Process { get; set; }
        public int NextRestartDay { get; set; }
        public bool RestartDone { get; set; }
    }
}
