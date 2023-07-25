using Microsoft.VisualBasic;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

using Timer = System.Windows.Forms.Timer;

namespace ConanServerLauncher
{
    internal class ServerManager
    {
        private Settings _settings;
        private ConcurrentDictionary<string, RunningProcessInformation> _runningProcesses;
        private Timer _processWatrchTimer;

        public ServerManager() 
        {
            _settings = new Settings();
            _runningProcesses = new ConcurrentDictionary<string, RunningProcessInformation>();
            _processWatrchTimer = new Timer();
            _processWatrchTimer.Interval = 60 * 1000;
            _processWatrchTimer.Tick += _processWatchTimer_Tick;
            _processWatrchTimer.Start();
            LoadSettings();
        }

        private void _processWatchTimer_Tick(Object? sender, EventArgs e)
        {
            var now = DateTime.Now;

            foreach (var server in _runningProcesses)
            {
                var configForServerProcess = _settings.ServerConfigurations.Where(sc => sc.Name == server.Key).First();

                var process = server.Value.Process;
                if (process.HasExited)
                {
                    process.Start();
                }
                else
                {
                    SetPriorityOfChildren(process, configForServerProcess.ProcessPriority);
                }

                var times = configForServerProcess.RestartTime.Split(';');
                foreach(var time in times)  
                {
                    var timeSplit = time.Split(':');
                    var hour = Int32.Parse(timeSplit[0]);
                    var minute = Int32.Parse(timeSplit[1]);

                    if(now.Hour == hour && now.Minute == minute)
                    {
                        Restart(server.Key);
                    }
                    else
                    {
                        var resultDateTime = new DateTime(now.Date.Year, now.Date.Month, now.Date.Day, hour, minute, 0);
                        var resultDateTimeMinus5Minutes = resultDateTime.AddMinutes(-5);

                        if(now.Hour == resultDateTimeMinus5Minutes.Hour && now.Minute == resultDateTimeMinus5Minutes.Minute)
                        {
                            SendRconMessage(server.Key);
                        }
                    }
                }
            }
        }

        private void LoadSettings()
        {
            var jsonText = File.ReadAllText("settings.json");
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            _settings = JsonSerializer.Deserialize<Settings>(jsonText, options);
        }

        public bool LoadServers()
        {
            var serverDirectory = Path.Combine(Directory.GetCurrentDirectory(), _settings.ServerFolder);
            var directories = Directory.EnumerateDirectories(serverDirectory);

            foreach (var d in directories)
            {
                var name = new DirectoryInfo(d).Name;
                if (_settings.ServerConfigurations.Any(sc => sc.Name == name) == false)
                {
                    _settings.ServerConfigurations.Add(new ServerConfiguration()
                    {
                        Name = name
                    });
                }
            }

            return true;
        }

        public void StartAndWatchProcess(string name)
        {
            if (_runningProcesses.ContainsKey(name) == false)
            {
                var serverConfig = _settings.ServerConfigurations.FirstOrDefault(sc => sc.Name == name);

                var startInfo = new ProcessStartInfo()
                {
                    Arguments = serverConfig.LaunchOptions,
                    FileName = Path.Combine(Directory.GetCurrentDirectory(), _settings.ServerFolder, name, "ConanSandboxServer.exe")
                };

                var p = Process.Start(startInfo);
                SetPriorityOfChildren(p, serverConfig.ProcessPriority);

                var processInformation = new RunningProcessInformation();
                processInformation.Process = p;
                processInformation.NextRestartDay = DateTime.Today.Day;

                if (_runningProcesses.TryAdd(name, processInformation) == false)
                {
                    Thread.Sleep(1000);
                    _runningProcesses.TryAdd(name, processInformation);
                }
            }
        }

        public void Restart(string name)
        {
            if (_runningProcesses.ContainsKey(name))
            {
                KillProcessAndChildrens(_runningProcesses[name].Process);
            }
        }

        public void StopAndStopWatching(string name)
        {
            if (_runningProcesses.ContainsKey(name))
            {
                KillProcessAndChildrens(_runningProcesses[name].Process);
                if(_runningProcesses.Remove(name, out _) == false)
                {
                    Thread.Sleep(1000);
                    _runningProcesses.Remove(name, out _);
                }
            }
        }

        private void SetPriorityOfChildren(Process p, ProcessPriorityClass priority)
        {
            var processSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + p.Id);
            var processCollection = processSearcher.Get();

            if(p.PriorityClass != priority)
                p.PriorityClass = priority;

            if (processCollection != null)
            {
                foreach (var mo in processCollection)
                {
                    var process = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));

                    if(process.PriorityClass != priority)
                        process.PriorityClass = priority;
                }
            }
        }

        private void KillProcessAndChildrens(Process p)
        {
            var processSearcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + p.Id);
            var processCollection = processSearcher.Get();

            try
            {
                if (!p.HasExited) p.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }

            if (processCollection != null)
            {
                foreach (var mo in processCollection)
                {
                    var process = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                    KillProcessAndChildrens(process);
                }
            }
        }

        public void SaveConfiguration(List<ServerConfiguration> serverConfigurations)
        {
            foreach (var serverConfiguration in serverConfigurations) 
            {
                var currentConfig = _settings.ServerConfigurations.FirstOrDefault(sc => sc.Name == serverConfiguration.Name);
                if(currentConfig != null)
                {
                    currentConfig.LaunchOptions = serverConfiguration.LaunchOptions;
                    currentConfig.ProcessPriority = serverConfiguration.ProcessPriority;
                    currentConfig.RestartTime = serverConfiguration.RestartTime;
                }
            }

            var jsonText = JsonSerializer.Serialize(_settings);
            File.WriteAllText("settings.json", jsonText);
        }

        public List<ServerConfiguration> GetServers()
        {
            return _settings.ServerConfigurations;
        }

        public async void SendRconMessage(string name)
        {
            try
            {
                var rconBatch = Path.Combine(Directory.GetCurrentDirectory(), _settings.ServerFolder, name, "Rcon", "SendRestartWarning.bat");
                var execDirectory = Path.GetDirectoryName(rconBatch);
                if (_runningProcesses.ContainsKey(name))
                {
                    var startInfo = new ProcessStartInfo()
                    {
                        FileName = "cmd.exe",
                        Arguments = "/c " + rconBatch,
                        WorkingDirectory = execDirectory,
                        RedirectStandardInput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    var rconProcess = new Process();
                    rconProcess.StartInfo = startInfo;
                    rconProcess.Start();

                    await Task.Delay(2000);

                    rconProcess.Kill();
                    rconProcess.WaitForExit();
                    rconProcess.Close();
                }
            }
            catch { }
        }
    }
}
