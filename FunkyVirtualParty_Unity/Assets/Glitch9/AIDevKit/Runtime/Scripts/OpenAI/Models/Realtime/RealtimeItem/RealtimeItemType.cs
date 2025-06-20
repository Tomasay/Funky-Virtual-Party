using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.OpenAI.Realtime
{
    public enum RealtimeItemType
    {
        [ApiEnum("Message", "message")]
        Message,

        [ApiEnum("FunctionCall", "function_call")]
        FunctionCall,

        [ApiEnum("FunctionCallOutput", "function_call_output")]
        FunctionCallOutput
    }
}
