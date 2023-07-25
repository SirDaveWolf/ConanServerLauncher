using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConanServerLauncher
{
    internal class Settings
    {
        public string ServerFolder { get; set; }
        public List<ServerConfiguration> ServerConfigurations { get; set; }
    }
}
