using System.Diagnostics;

namespace ConanServerLauncher
{
    public partial class Form1 : Form
    {
        private ServerManager _serverManager;
        private bool _saved;

        public Form1()
        {
            InitializeComponent();

            _serverManager = new ServerManager();
        }

        private void Form1_Load(Object sender, EventArgs e)
        {
            _serverManager.LoadServers();
            var serverConfigs = _serverManager.GetServers();

            var groupBoxes = new List<GroupBox>();
            foreach (var serverConfig in serverConfigs)
            {
                var groupBox = new GroupBox()
                {
                    Name = serverConfig.Name + "GroupBox",
                    Text = serverConfig.Name,
                    Location = new Point(12, 12 * (groupBoxes.Count + 1) + 100 * groupBoxes.Count),
                    Size = new Size(760, 100),
                    Tag = serverConfig.Name
                };

                groupBox.Controls.Add(new TextBox()
                {
                    Name = serverConfig.Name + "LaunchOptions",
                    Text = serverConfig.LaunchOptions,
                    Location = new Point(6, 22),
                    Size = new Size(748, 23)
                });

                var startButton = new Button()
                {
                    Text = "Start",
                    Location = new Point(679, 51),
                    Size = new Size(75, 23),
                    Tag = serverConfig.Name
                };

                var restartButton = new Button()
                {
                    Text = "Restart",
                    Location = new Point(592, 51),
                    Size = new Size(75, 23),
                    Tag = serverConfig.Name
                };

                var stopButton = new Button()
                {
                    Text = "Stop",
                    Location = new Point(505, 51),
                    Size = new Size(75, 23),
                    Tag = serverConfig.Name
                };

                var rconButton = new Button()
                {
                    Text = "Rcon",
                    Location = new Point(448, 51),
                    Size = new Size(45, 23),
                    Tag = serverConfig.Name
                };

                startButton.Click += new EventHandler(buttonStartClick);
                restartButton.Click += new EventHandler(buttonRestartClick);
                stopButton.Click += new EventHandler(buttonStopClick);
                rconButton.Click += new EventHandler(RconButton_Click);

                groupBox.Controls.Add(startButton);
                groupBox.Controls.Add(restartButton);
                groupBox.Controls.Add(stopButton);
                groupBox.Controls.Add(rconButton);

                var priorityComboBox = new ComboBox()
                {
                    Name = serverConfig.Name + "Priority",
                    Location = new Point(6, 52),
                    Size = new Size(121, 23)
                };

                priorityComboBox.Items.AddRange(Enum.GetNames(typeof(ProcessPriorityClass)));
                priorityComboBox.SelectedItem = serverConfig.ProcessPriority == 0 ? Enum.GetName(ProcessPriorityClass.Normal) : Enum.GetName(serverConfig.ProcessPriority);

                groupBox.Controls.Add(priorityComboBox);
                groupBox.Controls.Add(new TextBox()
                {
                    Name = serverConfig.Name + "RestartTime",
                    Text = serverConfig.RestartTime ?? "06:00",
                    Location = new Point(139, 52),
                    Size = new Size(300, 23)
                });

                groupBoxes.Add(groupBox);
            }

            foreach (var groupBox in groupBoxes)
                Controls.Add(groupBox);
        }

        private void RconButton_Click(Object? sender, EventArgs e)
        {
            if (_saved == false)
            {
                MessageBox.Show("The configuration needs to be saved first!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tag = (sender as Button).Tag as string;
            _serverManager.SendRconMessage(tag);
        }

        private void buttonSaveConfig_Click(Object sender, EventArgs e)
        {
            var serverConfigurations = new List<ServerConfiguration>();
            foreach (var control in Controls)
            {
                if (control.GetType() == typeof(GroupBox))
                {
                    var groupBox = (GroupBox)control;
                    var newConfig = new ServerConfiguration();
                    newConfig.Name = groupBox.Tag as string;
                    foreach (var innerControl in groupBox.Controls)
                    {
                        if (innerControl.GetType() == typeof(TextBox))
                        {
                            var innerTextBox = (TextBox)innerControl;
                            if (innerTextBox.Name.Contains("RestartTime"))
                            {
                                newConfig.RestartTime = innerTextBox.Text;
                            }
                            else if (innerTextBox.Name.Contains("LaunchOptions"))
                            {
                                newConfig.LaunchOptions = innerTextBox.Text;
                            }
                        }

                        if (innerControl.GetType() == typeof(ComboBox))
                        {
                            var comboBox = (ComboBox)innerControl;
                            newConfig.ProcessPriority = Enum.Parse<ProcessPriorityClass>(comboBox.SelectedItem.ToString());
                        }
                    }

                    serverConfigurations.Add(newConfig);
                }
            }

            _serverManager.SaveConfiguration(serverConfigurations);

            MessageBox.Show("Config saved!", Text);
            _saved = true;
        }

        private void buttonStartClick(Object sender, EventArgs e)
        {
            if (_saved == false)
            {
                MessageBox.Show("The configuration needs to be saved first!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tag = (sender as Button).Tag as string;
            _serverManager.StartAndWatchProcess(tag);
        }

        private void buttonRestartClick(Object sender, EventArgs e)
        {
            if (_saved == false)
            {
                MessageBox.Show("The configuration needs to be saved first!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tag = (sender as Button).Tag as string;
            _serverManager.Restart(tag);
        }

        private void buttonStopClick(Object sender, EventArgs e)
        {
            if (_saved == false)
            {
                MessageBox.Show("The configuration needs to be saved first!", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var tag = (sender as Button).Tag as string;
            _serverManager.StopAndStopWatching(tag);
        }

        private void buttonRefresh_Click(Object sender, EventArgs e)
        {
            for (var i = 0; i < Controls.Count; i++)
            {
                var control = Controls[i];
                if (control.GetType() == typeof(GroupBox))
                {
                    Controls.Remove(control);
                    i++;
                }
            }

            _serverManager.LoadServers();

            var serverConfigs = _serverManager.GetServers();

            var groupBoxes = new List<GroupBox>();
            foreach (var serverConfig in serverConfigs)
            {
                var groupBox = new GroupBox()
                {
                    Name = serverConfig.Name + "GroupBox",
                    Text = serverConfig.Name,
                    Location = new Point(12, 12 * (groupBoxes.Count + 1) + 100 * groupBoxes.Count),
                    Size = new Size(760, 100),
                    Tag = serverConfig.Name
                };

                groupBox.Controls.Add(new TextBox()
                {
                    Name = serverConfig.Name + "LaunchOptions",
                    Text = serverConfig.LaunchOptions,
                    Location = new Point(6, 22),
                    Size = new Size(748, 23)
                });

                var startButton = new Button()
                {
                    Text = "Start",
                    Location = new Point(679, 51),
                    Size = new Size(75, 23),
                    Tag = serverConfig.Name
                };

                var restartButton = new Button()
                {
                    Text = "Restart",
                    Location = new Point(592, 51),
                    Size = new Size(75, 23),
                    Tag = serverConfig.Name
                };

                var stopButton = new Button()
                {
                    Text = "Stop",
                    Location = new Point(505, 51),
                    Size = new Size(75, 23),
                    Tag = serverConfig.Name
                };

                var rconButton = new Button()
                {
                    Text = "Rcon",
                    Location = new Point(448, 51),
                    Size = new Size(45, 23),
                    Tag = serverConfig.Name
                };

                startButton.Click += new EventHandler(buttonStartClick);
                restartButton.Click += new EventHandler(buttonRestartClick);
                stopButton.Click += new EventHandler(buttonStopClick);
                rconButton.Click += new EventHandler(RconButton_Click);

                groupBox.Controls.Add(startButton);
                groupBox.Controls.Add(restartButton);
                groupBox.Controls.Add(stopButton);
                groupBox.Controls.Add(rconButton);

                var priorityComboBox = new ComboBox()
                {
                    Name = serverConfig.Name + "Priority",
                    Location = new Point(6, 52),
                    Size = new Size(121, 23)
                };

                priorityComboBox.Items.AddRange(Enum.GetNames(typeof(ProcessPriorityClass)));
                priorityComboBox.SelectedItem = serverConfig.ProcessPriority == 0 ? Enum.GetName(ProcessPriorityClass.Normal) : Enum.GetName(serverConfig.ProcessPriority);

                groupBox.Controls.Add(priorityComboBox);
                groupBox.Controls.Add(new TextBox()
                {
                    Name = serverConfig.Name + "RestartTime",
                    Text = serverConfig.RestartTime ?? "06:00",
                    Location = new Point(139, 52),
                    Size = new Size(300, 23)
                });

                groupBoxes.Add(groupBox);
            }

            foreach (var groupBox in groupBoxes)
                Controls.Add(groupBox);

            _saved = false;
        }
    }
}