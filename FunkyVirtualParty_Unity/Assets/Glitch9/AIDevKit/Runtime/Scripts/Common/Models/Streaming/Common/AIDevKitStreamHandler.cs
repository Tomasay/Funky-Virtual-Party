
using System;
using System.Collections.Generic;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit
{
    public abstract class AIDevKitStreamHandler<TSelf, TChunk, TData, TTask> : ITextStreamHandler
        where TSelf : AIDevKitStreamHandler<TSelf, TChunk, TData, TTask>
        where TChunk : AIDevKitChunk<TData>
        where TData : class, IGeneratedResult
        where TTask : class, IGENTask
    {
        public Action onStart;
        public Action<TData> onReceiveData;
        public Action<TData> onDone;
        public Action<string> onError;
        protected Action<TData> _systemCallback; // system callback for internal use

        protected TTask _task;
        protected Func<string, IEnumerable<TChunk>> _chunkFactory;
        protected TChunk _lastChunk;
        protected bool _isDone = false;

        internal abstract void ProcessChunk(TChunk chunk);
        internal abstract TData BuildFinalResult();

        public TSelf Initialize(Func<string, IEnumerable<TChunk>> chunkFactory, TTask task = null)
        {
            _task = task;
            _chunkFactory = chunkFactory;
            return this as TSelf;
        }

        public TSelf SetSystemCallback(Action<TData> systemCallback)
        {
            _systemCallback = systemCallback;
            return this as TSelf;
        }

        public void OnStart() => onStart?.Invoke();
        public void OnReceiveData(string streamedText)
        {
            //GNDebug.Mark(streamData); 
            if (_chunkFactory == null) throw new ArgumentNullException(nameof(_chunkFactory));
            if (_isDone || string.IsNullOrEmpty(streamedText)) return;

            foreach (TChunk chunk in _chunkFactory(streamedText))
            {
                if (chunk == null) continue;

                if (chunk.IsError)
                {
                    OnError(chunk.ErrorMessage);
                    return;
                }

                if (chunk.IsDone)
                {
                    OnDone();
                    return;
                }

                _lastChunk = chunk;
                ProcessChunk(chunk);
            }
        }

        public void OnDone()
        {
            if (_isDone) return;
            _isDone = true;

            TData lastDelta = BuildFinalResult();

            if (_task != null && _task.enableHistory)
                PromptRecordFactory.Create(_task, lastDelta);

            _systemCallback?.Invoke(lastDelta);
            onDone?.Invoke(lastDelta);
        }

        public void OnError(string error)
        {
            if (_isDone) return;
            _isDone = true;
            var formattedError = AIDevKitDebug.FormatErrorMessage(error);
            AIDevKitDebug.Error(formattedError);
            onError?.Invoke(formattedError);
            _systemCallback?.Invoke(null); // Notify that the stream is done with null result
            onDone?.Invoke(null); // Notify that the stream is done with null result
        }

        public void OnProgress(float progress)
        {
            // do nothing
        }

        public abstract class StreamHandlerBuilder<TBuilder>
            where TBuilder : StreamHandlerBuilder<TBuilder>
        {
            protected readonly TSelf handler = (TSelf)Activator.CreateInstance(typeof(TSelf));

            public TBuilder SetOnStart(Action onStart)
            {
                handler.onStart = onStart;
                return (TBuilder)this;
            }

            public TBuilder SetOnReceiveData(Action<TData> onReceiveData)
            {
                handler.onReceiveData = onReceiveData;
                return (TBuilder)this;
            }

            public TBuilder SetOnDone(Action<TData> onDone)
            {
                handler.onDone = onDone;
                return (TBuilder)this;
            }

            public TBuilder SetOnError(Action<string> onError)
            {
                handler.onError = onError;
                return (TBuilder)this;
            }

            public abstract TSelf Build();
        }
    }
}