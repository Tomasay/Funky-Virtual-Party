using System;
using Glitch9.CoreLib.IO.Audio;

namespace Glitch9.IO.Networking.RESTApi
{
    public interface IStreamHandler
    {
        void OnStart();
        void OnError(string error);
        void OnProgress(float progress);
        void OnDone();
        bool GetProgressEnabled => true;
    }

    public interface ITextStreamHandler : IStreamHandler
    {
        void OnReceiveData(string streamedText);
    }

    public interface IBinaryStreamHandler : IStreamHandler
    {
        void OnReceiveData(byte[] streamedData);
    }

    public interface IAudioStreamHandler : IStreamHandler
    {
        AudioFormat AudioFormat { get; }
        void OnReceiveData(float[] streamedAudio);
    }

    public class TextStreamHandler : StreamHandler<string>, ITextStreamHandler
    {
        public TextStreamHandler(
            Action onReceiveTextStart = null,
            Action<string> onReceiveText = null,
            Action<string> onError = null,
            Action onReceiveTextDone = null) : base(onReceiveTextStart, onReceiveText, onError, null, onReceiveTextDone) { }
    }

    public class BinaryStreamHandler : StreamHandler<byte[]>, IBinaryStreamHandler
    {
        public BinaryStreamHandler(
            Action onReceiveDataStart = null,
            Action<byte[]> onReceiveData = null,
            Action<string> onError = null,
            Action<float> onProgress = null,
            Action onReceiveDataDone = null) : base(onReceiveDataStart, onReceiveData, onError, onProgress, onReceiveDataDone) { }
    }

    public class AudioStreamHandler : StreamHandler<float[]>, IAudioStreamHandler
    {
        public AudioFormat AudioFormat => audioFormat;
        private readonly AudioFormat audioFormat;
        public AudioStreamHandler(
            AudioFormat audioFormat,
            Action onStart = null,
            Action<float[]> onReceiveAudio = null,
            Action<string> onError = null,
            Action<float> onProgress = null,
            Action onDone = null) : base(onStart, onReceiveAudio, onError, onProgress, onDone)
        {
            this.audioFormat = audioFormat;
        }
    }

    public abstract class StreamHandler<T> : IStreamHandler
    {
        public Action onStart;
        public Action<T> onReceiveData;
        public Action<string> onError;
        public Action<float> onProgress;
        public Action onDone;
        public bool GetProgressEnabled => onProgress != null;

        public StreamHandler(
            Action onStart = null,
            Action<T> onStream = null,
            Action<string> onError = null,
            Action<float> onProgress = null,
            Action onDone = null)
        {
            this.onStart += onStart;
            this.onReceiveData += onStream;
            this.onError += onError;
            this.onProgress += onProgress;
            this.onDone += onDone;
        }

        public virtual void OnStart() => onStart?.Invoke();
        public virtual void OnReceiveData(T data) => onReceiveData?.Invoke(data);
        public virtual void OnError(string error)
        {
            onError?.Invoke(error);
            OnDone();
        }
        public virtual void OnProgress(float progress) => onProgress?.Invoke(progress);
        public virtual void OnDone() => onDone?.Invoke();
    }
}