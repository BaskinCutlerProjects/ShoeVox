using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Speech.Recognition;
using Microsoft.Research.Kinect.Audio;
using System.IO;
using Microsoft.Speech.AudioFormat;

namespace ShoeVox
{
    public class KinectRecognizer : Recognizer
    {
        #region Private Properties
        private KinectAudioSource _source;
        private RecognizerInfo _ri;
        private Stream _stream;
        private SpeechRecognitionEngine _engine;
        #endregion

        #region Constructor
        public KinectRecognizer() : base() { }
        #endregion

        #region IRecognizer Methods
        public override void StartListening()
        {
            _source = new KinectAudioSource();
            _source.FeatureMode = true;
            _source.AutomaticGainControl = false;
            _source.SystemMode = SystemMode.OptibeamArrayOnly;

            _stream = _source.Start();
            _engine.SetInputToAudioStream(_stream, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));
            _engine.RecognizeAsync(RecognizeMode.Multiple);
        }

        public override void StopListening()
        {
            if (null != _engine)
            {
                _source.Stop();
                _engine.RecognizeAsyncCancel();
                _engine.RecognizeAsyncStop();
                _source.Dispose();
            }
        }
        #endregion

        #region Recognizer Methods
        protected override void InitializeEngine()
        {
            _ri = GetKinectRecognizer();
            if (null == _ri)
            {
                throw new RecognizerInitializationFailureException("Unable to find a Kinect speech recognizer.");
            }

            _engine = new SpeechRecognitionEngine(_ri.Id);
            _engine.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_engine_SpeechRecognized);
            _engine.RecognizerUpdateReached += new EventHandler<RecognizerUpdateReachedEventArgs>(_engine_RecognizerUpdateReached);
        }

        protected override void DisposeEngine()
        {
            _engine.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(_engine_SpeechRecognized);
            _engine.Dispose();
            _stream.Dispose();
            _source.Dispose();
        }

        protected override void BuildGrammar()
        {
            Choices choices = new Choices(_commands.ToArray());
            GrammarBuilder gb = new GrammarBuilder(_prefix);
            gb.Culture = _ri.Culture; 
            gb.Append(choices);
            Grammar g = new Grammar(gb);

            // ask the SRE to pause to update configuration, and then reload the grammar in the callback
            _engine.RequestRecognizerUpdate(new RecognizerUpdateGrammarRequest(g));
        }
        #endregion

        #region Private Methods
        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        void _engine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            SpeechRecognized(e.Result.Words.Select(x => x.Text), e.Result.Confidence);
        }

        void _engine_RecognizerUpdateReached(object sender, RecognizerUpdateReachedEventArgs e)
        {
            RecognizerUpdateGrammarRequest req = e.UserToken as RecognizerUpdateGrammarRequest;
            if (null == req)
                return;

            _engine.UnloadAllGrammars();
            _engine.LoadGrammar(req.Grammar);
        }
        #endregion

        #region Static Public Methods
        public static bool CanUseKinect()
        {
            return Microsoft.Research.Kinect.Nui.Runtime.Kinects.Count > 0;
        }
        #endregion

        #region RecognizerUpdateGrammarRequest
        /// <summary>
        /// POCO used as event token for SRE update request callbacks.
        /// </summary>
        private class RecognizerUpdateGrammarRequest
        {
            public Grammar Grammar { get; private set; }

            public RecognizerUpdateGrammarRequest(Grammar grammar)
            {
                Grammar = grammar;
            }
        }
        #endregion
    }
}
