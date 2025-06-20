using UnityEngine.Networking;

namespace Glitch9.IO.Networking.RESTApi
{
    public abstract class BaseStreamHandlerBuffer<T> : DownloadHandlerScript
        where T : class, IStreamHandler
    {
        protected readonly T _streamHandler;
        protected readonly RESTClient _client;
        protected bool _ignoreLogs = false;
        private bool _isFirstStream = false;

        public BaseStreamHandlerBuffer(RESTClient client, T streamHandler, bool ignoreLogs) : base()
        {
            _streamHandler = streamHandler;
            _client = client;
            _ignoreLogs = ignoreLogs;
        }

        /// <summary>
        /// Implement if needed to report progress
        /// </summary>
        /// <returns></returns>
        protected override float GetProgress()
        {
            if (_streamHandler.GetProgressEnabled)
            {
                float progress = base.GetProgress();
                _streamHandler.OnProgress(progress);
                return progress;
            }
            return 0f;
        }

        /// <summary>
        /// This method is called whenever data is received
        /// </summary>
        /// <param name="streamedData"></param>
        /// <param name="dataLength"></param>
        /// <returns></returns>
        protected override bool ReceiveData(byte[] streamedData, int dataLength)
        {
            if (streamedData == null || dataLength == 0) return false;

            if (_isFirstStream)
            {
                // This is the first chunk of data received, so we can initialize the stream handler
                _streamHandler.StartStreaming();
                _isFirstStream = false;
            }

            return ProcessData(streamedData, dataLength);
        }

        protected abstract bool ProcessData(byte[] streamedData, int dataLength);

        /// <summary>
        /// Called when all data has been received
        /// </summary>
        protected override void CompleteContent() => _streamHandler.FinishStreaming();
    }
}