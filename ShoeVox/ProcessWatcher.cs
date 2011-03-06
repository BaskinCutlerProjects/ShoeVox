using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;

namespace ShoeVox
{
    public enum ProcessEventType { Start, Stop };

    public class ProcessEventArgs : EventArgs
    {
        private string _processName;
        public string ProcessName { get { return _processName; } }

        private int _processId;
        public int ProcessId { get { return _processId; } }

        private ProcessEventType _eventType;
        public ProcessEventType EventType { get { return _eventType; } }

        public ProcessEventArgs(string processName, int processId, ProcessEventType eventType)
        {
            _processName = processName;
            _processId = processId;
            _eventType = eventType;
        }
    }

    class ProcessWatcher : IDisposable
    {
        public event EventHandler<ProcessEventArgs> ProcessEvent;

        private Dictionary<string, ManagementEventWatcher> _watchers;

        private readonly string QUERY_TEMPLATE = "SELECT * FROM {0} WITHIN {1} WHERE TargetInstance ISA 'Win32_Process' AND ({2})";
        private readonly string QUERY_TARGET_TEMPLATE = "TargetInstance.Name = '{0}.exe'";
        private readonly string PROCESS_START = "__InstanceCreationEvent";
        private readonly string PROCESS_END = "__InstanceDeletionEvent";

        public ProcessWatcher() 
        {
            _watchers = new Dictionary<string, ManagementEventWatcher>();
        }

        public void Watch(List<string> processNames, int timeThreshold=5)
        {
            // discover which processes were running before watch was called and act as if they just started by raising an event
            Process[] processes = Process.GetProcesses();
            var procs = from p in processes
                        where processNames.Contains(p.ProcessName)
                        orderby p.ProcessName ascending
                        select p;
            foreach (Process process in procs.ToList())
            {
                OnProcessEvent(new ProcessEventArgs(process.ProcessName, process.Id, ProcessEventType.Start));
            }

            // start listening for future process events
            doWatch(PROCESS_START, timeThreshold, processNames);
            doWatch(PROCESS_END, timeThreshold, processNames);
        }

        private void doWatch(string eventName, int timeThreshold, List<string> processNames)
        {
            // if a watcher for this event was previously created, stop it before attempting to create a new one
            if (_watchers.ContainsKey(eventName))
            {
                _watchers[eventName].Stop();
                _watchers[eventName].EventArrived -= new EventArrivedEventHandler(watcher_EventArrived);
                _watchers.Remove(eventName);
            }

            WqlEventQuery query = new WqlEventQuery(
                String.Format(QUERY_TEMPLATE,
                    eventName,
                    timeThreshold,
                    String.Join(" OR ",
                                processNames.Select(x => String.Format(QUERY_TARGET_TEMPLATE, x)).ToArray())
                )
            );

            ManagementEventWatcher watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(watcher_EventArrived);
            watcher.Start();

            _watchers.Add(eventName, watcher);
        }

        private void watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
            string processName = targetInstance.Properties["Name"].Value.ToString();
            uint processId = (uint)targetInstance.Properties["ProcessId"].Value;
            string eventName = e.NewEvent.ClassPath.ClassName;

            if (eventName == PROCESS_START)
            {
                OnProcessEvent(new ProcessEventArgs(processName, (int)processId, ProcessEventType.Start));
            }
            else if (eventName == PROCESS_END)
            {
                OnProcessEvent(new ProcessEventArgs(processName, (int)processId, ProcessEventType.Stop));
            }
        }

        protected virtual void OnProcessEvent(ProcessEventArgs e)
        {
            EventHandler<ProcessEventArgs> handler = ProcessEvent;
            if (null != handler)
            {
                handler(this, e);
            }
        }

        public void Dispose()
        {
            // disconnect any remaining watchers so that COM handles can be released
            foreach (ManagementEventWatcher watcher in _watchers.Values)
            {
                watcher.Stop();
                watcher.EventArrived -= new EventArrivedEventHandler(watcher_EventArrived);
                watcher.Dispose();
            }
            _watchers.Clear();
        }
    }
}
