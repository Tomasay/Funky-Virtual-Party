namespace Glitch9.IO.Networking.RESTApi
{
    public class BinaryStreamHandlerBuffer : BaseStreamHandlerBuffer<BinaryStreamHandler>
    {
        public BinaryStreamHandlerBuffer(RESTClient client, BinaryStreamHandler binaryStreamHandler, bool ignoreLogs) : base(client, binaryStreamHandler, ignoreLogs)
        {
        }

        protected override bool ProcessData(byte[] streamedData, int dataLength)
        {
            _streamHandler.OnReceiveStreamedData(streamedData);
            return true;
        }
    }
}