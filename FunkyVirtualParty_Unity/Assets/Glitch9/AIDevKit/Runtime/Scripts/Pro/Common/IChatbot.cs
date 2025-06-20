using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace Glitch9.AIDevKit
{
    public interface IChatbot
    {
        string Name { get; }
        bool Stream { get; }
        Model Model { get; }
        List<ChatMessage> Messages { get; }
        UniTask<ChatMessage> EnterChatAsync(ChatMessage inputMessage);
    }
}