using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;

namespace ShoeVox
{
    /// <summary>
    /// Speech recognition engine based on the built-in .NET SpeechRecognitionEngine.
    /// 
    /// http://msdn.microsoft.com/en-us/library/system.speech.recognition.speechrecognitionengine.aspx
    /// </summary>
    public class SystemRecognizer : Recognizer
    {
        #region Private Properties
        private SpeechRecognitionEngine _engine;
        #endregion

        #region Constructor
        public SystemRecognizer() : base() { }
        #endregion

        #region IRecognizer Methods
        public override void StartListening()
        {
            _engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public override void StopListening()
        {
            _engine.RecognizeAsyncStop();
        }
        #endregion

        #region Recognizer Methods
        protected override void InitializeEngine()
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

        protected override void DisposeEngine()
        {
            _engine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(_engine_SpeechRecognized);
            _engine.Dispose();
        }

        protected override void BuildGrammar()
        {
            _engine.UnloadAllGrammars();
            Choices choices = new Choices(_commands.ToArray());
            GrammarBuilder gb = new GrammarBuilder(_prefix);
            gb.Append(choices);
            Grammar g = new Grammar(gb);
            _engine.LoadGrammar(g);
        }
        #endregion

        #region Private methods
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
