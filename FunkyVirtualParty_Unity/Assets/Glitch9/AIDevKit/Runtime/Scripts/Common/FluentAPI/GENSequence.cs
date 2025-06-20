using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;

namespace Glitch9.AIDevKit
{
    public class GENSequence
    {
        private class GENSequenceOutput
        {
            internal string text;
            internal Texture2D image;
            internal AudioClip audio;
        }

        private enum GENSequenceType { None, Text, Image, Audio }
        private readonly List<(GENSequenceType, Func<GENSequenceOutput, IGENTask>)> tasks = new();
        private GENSequenceOutput _currentOutput = new();

        public GENSequence AppendInterval(float seconds)
        {
            tasks.Add((GENSequenceType.None, _ => new GENDelay(seconds)));
            return this;
        }

        public GENSequence AppendText(IGENTask nextTask)
        {
            tasks.Add((GENSequenceType.Text, _ => nextTask));
            return this;
        }

        public GENSequence AppendImage(IGENTask nextTask)
        {
            tasks.Add((GENSequenceType.Image, _ => nextTask));
            return this;
        }

        public GENSequence AppendAudio(IGENTask nextTask)
        {
            tasks.Add((GENSequenceType.Audio, _ => nextTask));
            return this;
        }

        public GENSequence AppendTextToText(Func<string, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Text, output => nextTask(output.text)));
            return this;
        }

        public GENSequence AppendTextToImage(Func<string, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Image, output => nextTask(output.text)));
            return this;
        }

        public GENSequence AppendTextToAudio(Func<string, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Audio, output => nextTask(output.text)));
            return this;
        }

        public GENSequence AppendImageToText(Func<Texture2D, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Text, output => nextTask(output.image)));
            return this;
        }

        public GENSequence AppendImageToImage(Func<Texture2D, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Image, output => nextTask(output.image)));
            return this;
        }

        public GENSequence AppendAudioToText(Func<AudioClip, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Text, output => nextTask(output.audio)));
            return this;
        }

        public GENSequence AppendAudioToAudio(Func<AudioClip, IGENTask> nextTask)
        {
            tasks.Add((GENSequenceType.Audio, output => nextTask(output.audio)));
            return this;
        }

        public async UniTask ExecuteAsync()
        {
            if (tasks.Count == 0) throw new InvalidOperationException("GENSequence has no tasks to execute.");
            foreach ((GENSequenceType type, Func<GENSequenceOutput, IGENTask> task) in tasks)
            {
                switch (type)
                {
                    case GENSequenceType.Text:
                        var textTask = task(_currentOutput);
                        _currentOutput.text = await textTask.ExecuteAsync<string>();
                        break;
                    case GENSequenceType.Image:
                        var imageTask = task(_currentOutput);
                        _currentOutput.image = await imageTask.ExecuteAsync<Texture2D>();
                        break;
                    case GENSequenceType.Audio:
                        var audioTask = task(_currentOutput);
                        _currentOutput.audio = await audioTask.ExecuteAsync<AudioClip>();
                        break;
                    case GENSequenceType.None:
                        var delayTask = task(_currentOutput);
                        await delayTask.ExecuteAsync<object>();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    internal class GENDelay : IGENTask
    {
        public bool enableHistory => false;
        public Model model => null;
        public string sender => null;
        public int n => -1;
        public Dictionary<string, object> options => null;
        public string outputPath => null;
        public bool saveOutput => false;
        public bool ignoreLogs => false;
        public string fileNote => null;
        public MIMEType outputMimeType => MIMEType.Unknown;
        public bool isWrapperTask => true;

        internal TimeSpan interval;
        internal GENDelay(TimeSpan interval) => this.interval = interval;
        internal GENDelay(float seconds) => interval = TimeSpan.FromSeconds(seconds);
        public void SetOption(string key, object value) => throw new NotImplementedException();
        public bool TryGetOption<T>(string key, out T value) => throw new NotImplementedException();

        public async UniTask<T> ExecuteAsync<T>()
        {
            await UniTask.Delay(interval);
            return default;
        }
    }
}

