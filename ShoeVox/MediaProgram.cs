using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace ShoeVox
{
    class MediaProgram
    {
        public string Name;
        public string ProcessName;
        public Dictionary<string, string> Commands = new Dictionary<string, string>();

        public MediaProgram() { }

        public MediaProgram(string name, string processName)
        {
            Name = name;
            ProcessName = processName;
        }
    }

    class MediaPrograms
    {
        private Dictionary<string, MediaProgram> _programs = new Dictionary<string, MediaProgram>();
        public List<string> ProcessNames
        {
            get
            {
                return _programs.Keys.ToList();
            }
        }

        private HashSet<string> _commandPhrases = new HashSet<string>();
        public HashSet<string> CommandPhrases { get { return _commandPhrases; } }

        public MediaPrograms() { }

        public string GetPrettyNameForProcess(string processName)
        {
            MediaProgram program;
            if (_programs.TryGetValue(processName, out program))
            {
                return program.Name;
            }
            return String.Empty;
        }

        public string GetKeysForProcessCommand(string processName, string command)
        {
            MediaProgram program;
            if (_programs.TryGetValue(processName, out program))
            {
                string keys = "";
                if (program.Commands.TryGetValue(command, out keys))
                {
                    return keys;
                }
            }
            return String.Empty;
        }

        public void LoadFromXml(string filename)
        {
            _programs.Clear();
            _commandPhrases.Clear();

            // Read set of media programs from configuration XML file
            using (TextReader reader = new StreamReader(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(MediaXml.programs));
                MediaXml.programs mediaPrograms = (MediaXml.programs)serializer.Deserialize(reader);

                foreach (MediaXml.programsProgram program in mediaPrograms.Items)
                {
                    MediaProgram mediaProgram = new MediaProgram(program.name, program.process);

                    foreach (MediaXml.programsProgramCommandsCommand command in program.commands)
                    {
                        mediaProgram.Commands.Add(command.name, command.keys);
                        _commandPhrases.Add(command.name);
                    }

                    _programs.Add(mediaProgram.ProcessName, mediaProgram);
                }
            }
        }
    }
}
