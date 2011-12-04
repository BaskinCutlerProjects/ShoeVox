using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using WindowsInput;

namespace ShoeVox
{
    public partial class Form1 : Form
    {
        #region Fields and Properties
        string regKeyName = "ShoeVox";
        RegistryKey startupRegKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        string gitHubUrl = "http://github.com/maxcutler/ShoeVox";

        public string prefix
        {
            get
            {
                return Properties.Settings.Default.Prefix;
            }
            set
            {
                Properties.Settings.Default.Prefix = value;
                Properties.Settings.Default.Save();

                if (null != engine)
                    engine.SetPrefix(value);
            }
        }

        public IRecognizer engine;
        private bool _engineRunning;
        public bool EngineRunning {
            get { return _engineRunning; }
            set
            {
                bool isRunning = (bool)value;

                // reconcile old value with new one
                if (isRunning && !_engineRunning)
                {
                    engine.StartListening();
                }
                else if (!isRunning && _engineRunning)
                {
                    engine.StopListening();
                }
                _engineRunning = isRunning;
            }
        }

        private ProcessWatcher processWatcher;
        private MediaPrograms programs;

        // mapping of process IDs of running programs to their process name
        private Dictionary<int, string> runningPrograms = new Dictionary<int, string>();

        #endregion

        #region Initialization

        public Form1()
        {
            InitializeComponent();
            processWatcher = new ProcessWatcher();
            programs = new MediaPrograms();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Check the runOnStartup box if the reg key exists
            if (startupRegKey.GetValue(regKeyName) != null)
            {
                runOnStartupMenuItem.Checked = true;
            }

            //Read the MediaXML with process and grammar definitions
            try
            {
                string mediaXmlPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "media.xml");
                programs.LoadFromXml(mediaXmlPath);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Cannot find media.xml file in the current directory. Please create this file or download it again.");
                Close();
                return;
            }

            // Initialize speech recognition engine
            try
            {
                engine = new SystemRecognizer();
            }
            catch (RecognizerInitializationFailureException ex)
            {
                MessageBox.Show(ex.Message);
                Close();
                return;
            }
            engine.SetPrefix(prefix);
            engine.SetCommands(programs.CommandPhrases);
            engine.CommandRecognized += new EventHandler<CommandRecognizedEventArgs>(engine_CommandRecognized);

            //Start listening for speech
            EngineRunning = true;

            // Initialize the process watcher
            processWatcher.ProcessEvent += new EventHandler<ProcessEventArgs>(processWatcher_ProcessEvent);
            processWatcher.Watch(programs.ProcessNames);
        }
        #endregion

        #region Form Event Handlers

        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(gitHubUrl);
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listenMenuItem_Click(object sender, EventArgs e)
        {
            if (listenMenuItem.Checked)
            {
                EngineRunning = true;
            }
            else
            {
                EngineRunning = false;
            }
        }

        private void prefixMenuItem_Click(object sender, EventArgs e)
        {
            using (SetPrefixDialog prefixDialog = new SetPrefixDialog(prefix))
            {
                DialogResult result = prefixDialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    prefix = prefixDialog.GetPrefix;
                }
            }
        }

        private void runOnStartupMenuItem_Click(object sender, EventArgs e)
        {
            if (runOnStartupMenuItem.Checked)
            {
                startupRegKey.SetValue(regKeyName, "\"" + Application.ExecutablePath.ToString() + "\"");
            }
            else
            {
                startupRegKey.DeleteValue(regKeyName, false);
            }
        }

        #endregion

        #region Command Recognition
        void engine_CommandRecognized(object sender, CommandRecognizedEventArgs e)
        {
            List<Process> runningProcesses = new List<Process>();
            foreach (int pid in runningPrograms.Keys)
            {
                try
                {
                    Process p = Process.GetProcessById(pid);
                    runningProcesses.Add(p);
                }
                catch (ArgumentException)
                {
                    // process is no longer alive
                }
            }

            IntPtr currentForeground = NativeMethods.GetForegroundWindow();

            bool activeIsMedia = runningProcesses.Count(x => x.MainWindowHandle == currentForeground) > 0;

            foreach (Process process in runningProcesses)
            {
                if (programs.ProcessNames.Contains(process.ProcessName) &&
                    ((activeIsMedia && process.MainWindowHandle == currentForeground) ||
                        !activeIsMedia))
                {
                    string keys = programs.GetKeysForProcessCommand(process.ProcessName, e.Command);
                    if (!String.IsNullOrEmpty(keys))
                    {
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                        NativeMethods.SetFocus(process.MainWindowHandle);
                        System.Threading.Thread.Sleep(50);
                        if (keys == "{space}")
                        {
                            //SendKeys doesn't send space correctly, so use this
                            InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);
                            System.Threading.Thread.Sleep(50);
                        }
                        else
                        {
                            SendKeys.SendWait(keys);
                        }
                    }
                }
            }

            NativeMethods.SetForegroundWindow(currentForeground);

            if (Properties.Settings.Default.CommandConfirmationTooltip)
            {
                notifyIcon.ShowBalloonTip(100, "Command", e.Command, ToolTipIcon.Info);
            }
        }

        #endregion

        #region Process Watching

        void processWatcher_ProcessEvent(object sender, ProcessEventArgs e)
        {
            // extract the process name without the trailing .exe
            string processName = e.ProcessName;
            if (processName.EndsWith(".exe"))
            {
                processName = processName.Substring(0, processName.Length - 4);
            }

            switch (e.EventType)
            {
                case ProcessEventType.Start:
                    // add the new process to our list of running processes
                    if (!runningPrograms.ContainsKey(e.ProcessId))
                    {
                        runningPrograms.Add(e.ProcessId, processName);
                    }

                    // if the user has enabled listening for commands...
                    if (listenMenuItem.Checked)
                    {
                        // make sure the engine is now running
                        if (!EngineRunning)
                        {
                            EngineRunning = true;
                        }

                        // alert the user that this program will respond to commands
                        notifyIcon.ShowBalloonTip(500, "Listening", programs.GetPrettyNameForProcess(processName) + " has started and can be commanded by voice.", ToolTipIcon.Info);
                    }
                    break;

                case ProcessEventType.Stop:
                    // remove the process from our list of running processes
                    if (runningPrograms.ContainsKey(e.ProcessId))
                    {
                        runningPrograms.Remove(e.ProcessId);
                    }

                    // if there are no more running processes, turn off the SRE
                    if (runningPrograms.Count == 0 && EngineRunning)
                    {
                        EngineRunning = false;
                    }

                    break;
            }
        }

        #endregion
    }

    internal class NativeMethods
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        internal static extern IntPtr SetFocus(IntPtr hwnd);
    }
}