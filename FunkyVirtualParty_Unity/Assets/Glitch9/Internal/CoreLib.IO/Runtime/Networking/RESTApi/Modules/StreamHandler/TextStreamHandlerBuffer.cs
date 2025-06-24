using System;
using System.Text;


namespace Glitch9.IO.Networking.RESTApi
{
    public class TextStreamHandlerBuffer : BaseStreamHandlerBuffer<ITextStreamHandler>
    {
        /// <summary>
        /// Temporary buffer for storing incomplete characters between data chunks
        /// </summary>
        private byte[] _leftoverBytes;

        public TextStreamHandlerBuffer(RESTClient client, ITextStreamHandler streamHandler, bool ignoreLogs) : base(client, streamHandler, ignoreLogs)
        {
        }

        // protected override bool ProcessData(byte[] streamedData, int dataLength)
        // {
        //     byte[] fullDataChunk = streamedData;
        //     if (_leftoverBytes != null && _leftoverBytes.Length > 0)
        //     {
        //         // Combine leftover bytes from the previous chunk with the current chunk
        //         fullDataChunk = new byte[_leftoverBytes.Length + dataLength];
        //         Buffer.BlockCopy(_leftoverBytes, 0, fullDataChunk, 0, _leftoverBytes.Length);
        //         Buffer.BlockCopy(streamedData, 0, fullDataChunk, _leftoverBytes.Length, dataLength);
        //         _leftoverBytes = null;
        //     }

        //     // Try to convert the bytes to string
        //     string encodedText = Encoding.UTF8.GetString(fullDataChunk);
        //     if (!_ignoreLogs) _client.Logger.Stream(encodedText);

        //     // Here, you should invoke the onDataChunkReceived event or callback with the Text
        //     //_onDataChunkReceived?.Invoke(encodedText);
        //     _streamHandler.OnReceiveStreamedData(encodedText);

        //     // Check if the last byte potentially begins a multibyte character
        //     int lastFullCharIndex = GetLastFullCharIndex(fullDataChunk);
        //     if (lastFullCharIndex < fullDataChunk.Length - 1)
        //     {
        //         // Store incomplete character bytes to be processed with the next chunk
        //         int leftoverLength = fullDataChunk.Length - lastFullCharIndex - 1;
        //         _leftoverBytes = new byte[leftoverLength];
        //         Buffer.BlockCopy(fullDataChunk, lastFullCharIndex + 1, _leftoverBytes, 0, leftoverLength);
        //     }

        //     return true;
        // }

        protected override bool ProcessData(byte[] streamedData, int dataLength)
        {
            if (streamedData == null || dataLength <= 0)
                return false;

            try
            {
                byte[] fullDataChunk = streamedData;

                if (_leftoverBytes != null && _leftoverBytes.Length > 0)
                {
                    fullDataChunk = new byte[_leftoverBytes.Length + dataLength];
                    Buffer.BlockCopy(_leftoverBytes, 0, fullDataChunk, 0, _leftoverBytes.Length);
                    Buffer.BlockCopy(streamedData, 0, fullDataChunk, _leftoverBytes.Length, dataLength);
                    _leftoverBytes = null;
                }

                string encodedText = Encoding.UTF8.GetString(fullDataChunk);
                if (!_ignoreLogs) _client.Logger.Stream(encodedText);

                _streamHandler?.OnReceiveData(encodedText); // null check 보호

                int lastFullCharIndex = GetLastFullCharIndex(fullDataChunk);
                if (lastFullCharIndex < fullDataChunk.Length - 1)
                {
                    int leftoverLength = fullDataChunk.Length - lastFullCharIndex - 1;
                    _leftoverBytes = new byte[leftoverLength];
                    Buffer.BlockCopy(fullDataChunk, lastFullCharIndex + 1, _leftoverBytes, 0, leftoverLength);
                }

                return true;
            }
            catch (ObjectDisposedException ode)
            {
                UnityEngine.Debug.LogWarning($"[StreamBuffer] Disposed during processing: {ode.Message}");
                return false;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError($"[StreamBuffer] Exception in ProcessData: {ex}");
                return false;
            }
        }

        /// <summary>
        /// Utility method to find the index of the last full character in a byte array
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private int GetLastFullCharIndex(byte[] bytes)
        {
            int lastIndex = bytes.Length - 1;

            // Work backwards from the end of the byte array
            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                // Lead byte for UTF-8 multibyte character sequence will have two high bits set
                if ((bytes[i] & 0xC0) != 0x80)
                {
                    lastIndex = i;
                    break;
                }
            }

            return lastIndex;
        }
    }
}