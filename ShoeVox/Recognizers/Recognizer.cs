using System;
using System.Collections.Generic;
using System.Linq;

namespace ShoeVox
{
    #region Events
    public class CommandRecognizedEventArgs : EventArgs
    {
        private string _command;
        public string Command { get { return _command; } }

        public CommandRecognizedEventArgs(string command)
        {
            _command = command;
        }
    }
    #endregion

    #region Exceptions
    public class RecognizerInitializationFailureException : Exception
    {
        public RecognizerInitializationFailureException() { }

        public RecognizerInitializationFailureException(string message) : base(message) { }

        public RecognizerInitializationFailureException(string message, Exception innerException) : base(message, innerException) { }
    }
    #endregion

    #region Interfaces
    public interface IRecognizer : IDisposable
    {
        void SetPrefix(string prefix);
        void SetCommands(IEnumerable<string> commands);

        void StartListening();
        void StopListening();

        event EventHandler<CommandRecognizedEventArgs> CommandRecognized;
    }
    #endregion

    /// <summary>
    /// Base class for recognizers based on the standard Windows speech recognition API.
    /// </summary>
    public abstract class Recognizer : IRecognizer
    {
        #region Protected Properties
        protected string _prefix = String.Empty;
        protected IEnumerable<string> _commands = null;
        #endregion

        #region Constructor
        public Recognizer()
        {
            InitializeEngine();
        }
        #endregion

        #region IRecognizer Methods
        public void SetPrefix(string prefix)
        {
            _prefix = prefix;

            // if the command list was already set, we need to rebuild the grammar with the new prefix
            if (null != _commands)
                BuildGrammar();
        }

        public void SetCommands(IEnumerable<string> commands)
        {
            _commands = commands;
            BuildGrammar();
        }

        public abstract void StartListening();
        public abstract void StopListening();
        #endregion

        #region IRecognizer Events
        public event EventHandler<CommandRecognizedEventArgs> CommandRecognized;
        protected void OnCommandRecognized(string command)
        {
            if (null != CommandRecognized)
                CommandRecognized(this, new CommandRecognizedEventArgs(command));
        }
        #endregion

        #region IDispose Methods
        public void Dispose()
        {
            DisposeEngine();
        }
        #endregion

        #region Protected Methods
        protected abstract void InitializeEngine();
        protected abstract void DisposeEngine();

        /// <summary>
        /// Builds a SRE grammar based on the previously set prefix and command lists.
        /// </summary>
        protected abstract void BuildGrammar();
        #endregion
    }
}
