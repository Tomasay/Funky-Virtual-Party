using System;
using Glitch9.CoreLib.IO.Audio;

namespace Glitch9.IO.Networking.RESTApi
{
    public interface IStreamHandler
    {
        void StartStreaming();
        void OnError(string error);
        void OnProgress(float progress);
        void FinishStreaming();
        bool GetProgressEnabled => true;
    }

    public interface ITextStreamHandler : IStreamHandler
    {
        void OnReceiveStreamedData(string streamedText);
    }

    public interface IBinaryStreamHandler : IStreamHandler
    {
        void OnReceiveStreamedData(byte[] streamedData);
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

    public class PcmAudioStreamHandler : StreamHandler<float[]>
    {
        public readonly AudioFormat audioFormat;
        public readonly int headerSize; // size of the header in bytes
        public PcmAudioStreamHandler(
            AudioFormat audioFormat,
            int headerSize = 44, // 44 bytes for WAV header
            Action onStart = null,
            Action<float[]> onStream = null,
            Action<string> onError = null,
            Action<float> onProgress = null,
            Action onDone = null) : base(onStart, onStream, onError, onProgress, onDone)
        {
            this.audioFormat = audioFormat;
            this.headerSize = headerSize;
        }
    }

    public abstract class StreamHandler<T> : IStreamHandler
    {
        public Action onStart;
        public Action<T> onStream;
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
            this.onStream += onStream;
            this.onError += onError;
            this.onProgress += onProgress;
            this.onDone += onDone;
        }

        public virtual void StartStreaming() => onStart?.Invoke();
        public virtual void OnReceiveStreamedData(T data) => onStream?.Invoke(data);
        public virtual void OnError(string error)
        {
            onError?.Invoke(error);
            FinishStreaming();
        }
        public virtual void OnProgress(float progress) => onProgress?.Invoke(progress);
        public virtual void FinishStreaming() => onDone?.Invoke();
    }
}