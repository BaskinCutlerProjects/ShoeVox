using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Recognition;

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
    /// Speech recognition engine based on the built-in .NET SpeechRecognitionEngine.
    /// 
    /// http://msdn.microsoft.com/en-us/library/system.speech.recognition.speechrecognitionengine.aspx
    /// </summary>
    public sealed class Recognizer : IRecognizer
    {
        #region Private Properties
        private SpeechRecognitionEngine _engine;

        private string _prefix = String.Empty;
        private IEnumerable<string> _commands = null;
        #endregion

        #region Constructor
        public Recognizer()
        {
            _engine = new SpeechRecognitionEngine();

            try
            {
                _engine.SetInputToDefaultAudioDevice();
            }
            catch (InvalidOperationException ex)
            {
                throw new RecognizerInitializationFailureException("You must have a microphone attached to your system to use this application. Please attach a microphone and then restart this application.", ex);
            }

            _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_engine_SpeechRecognized);
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

        public void StartListening()
        {
            _engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public void StopListening()
        {
            _engine.RecognizeAsyncStop();
        }
        #endregion

        #region IRecognizer Events
        public event EventHandler<CommandRecognizedEventArgs> CommandRecognized;
        private void OnCommandRecognized(string command)
        {
            if (null != CommandRecognized)
                CommandRecognized(this, new CommandRecognizedEventArgs(command));
        }
        #endregion

        #region IDispose Methods
        public void Dispose()
        {
            _engine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(_engine_SpeechRecognized);
            _engine.Dispose();
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Builds a SRE grammar based on the previously set prefix and command lists.
        /// </summary>
        private void BuildGrammar()
        {
            _engine.UnloadAllGrammars();
            Choices choices = new Choices(_commands.ToArray());
            GrammarBuilder gb = new GrammarBuilder(_prefix);
            gb.Append(choices);
            Grammar g = new Grammar(gb);
            _engine.LoadGrammar(g);
        }

        private void _engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            // Ignore anything below the confidence threshold
            if (e.Result.Confidence < Properties.Settings.Default.ConfidenceThreshold)
            {
                return;
            }

            // Count number of words in the prefix
            int skip = _prefix.Split(' ').Count();

            // Strip the prefix from the recognized phrase to get the command
            string command = String.Join(" ", e.Result.Words.Skip(skip).Select(x => x.Text).ToArray());

            // Fire event
            OnCommandRecognized(command);
        }
        #endregion
    }
}
