using Glitch9.AIDevKit.Advanced.Chat;
using Glitch9.Editor.IMGUI;


namespace Glitch9.AIDevKit.Editor.Chatbots
{
    public class ChatbotManagerTreeViewItem : ExtendedTreeViewItem
        <
            ChatbotManagerTreeViewItem,
            ChatSession,
            ChatbotManagerTreeViewItemFilter
        >
    {
        internal string ChatbotName => Data?.Name ?? "Unknown";
        internal string ModelName => _modelName ??= Data?.Model.SafeGetName();
        internal string Instruction => Data?.Instructions ?? "No instruction set";
        internal UnixTime CreatedAt => Data?.CreatedAt ?? UnixTime.MinValue;
        internal UnixTime UpdatedAt => Data?.UpdatedAt ?? UnixTime.MinValue;
        internal string LastMessage => Data?.LastMessage ?? "-";
        private string _modelName;

        public ChatbotManagerTreeViewItem(int id, int depth, string displayName, ChatSession data) : base(id, depth, displayName, data) { }

        public override int CompareTo(ChatbotManagerTreeViewItem anotherItem, int columnIndex, bool ascending)
        {
            return columnIndex switch
            {
                ChatbotManagerWindow.ColumnIndex.NAME => CompareByString(ascending, anotherItem, data => data.Data.Name),
                ChatbotManagerWindow.ColumnIndex.MODEL => CompareByString(ascending, anotherItem, data => data.Data.Model),
                ChatbotManagerWindow.ColumnIndex.CREATED_AT => CompareByUnixTime(ascending, anotherItem, data => data.Data.CreatedAt),
                ChatbotManagerWindow.ColumnIndex.LAST_MESSAGE => CompareByString(ascending, anotherItem, data => data.Data.Instructions),
                ChatbotManagerWindow.ColumnIndex.UPDATED_AT => CompareByUnixTime(ascending, anotherItem, data => data.Data.UpdatedAt),
                _ => 0
            };
        }

        public override bool Search(string searchString)
        {
            if (Data == null) return false;
            if (string.IsNullOrEmpty(searchString)) return true;
            if (Data.Id != null && Data.Id.Contains(searchString)) return true;
            if (Data.Name != null && Data.Name.Contains(searchString)) return true;
            return false;
        }
    }
}