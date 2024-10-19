using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;
using UnityEngine.Events;

/// Modification of a Script by Alex Evgrashin (Macoron)

namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class RecordingHandler : InputSource
    {
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;
        
        private string _buffer;

        private void Awake()
        {   
            microphoneRecord.OnRecordStop += OnRecordStop;
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
        }
        
        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();
            
            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null) 
                return;

            var time = sw.ElapsedMilliseconds;
            var rate = recordedAudio.Length / (time * 0.001f);

            var text = res.Result;
            if (printLanguage)
                text += $"\n\nLanguage: {res.Language}";
            UnityEngine.Debug.Log("whisper says: " + text);
            // GM.Instance.AskQuestion(text);
            foreach (var s in res.Segments) {
                UnityEngine.Debug.Log("segmentStart: " + s.Start.ToString() + " content: " + s.Text);
            }
            requestObject.SetMessage(res);
            requestObject.CloseChannel();
        }

        protected override void SetupRecord()
        {
            microphoneRecord.StartRecord();
        }

        public override void EndRecord()
        {
            microphoneRecord.StopRecord();
        }
    }
}