using Glitch9.AIDevKit.OpenAI;
using Glitch9.Editor.IMGUI;


namespace Glitch9.AIDevKit.Editor.Assistants
{
    public class AssistantManagerTreeViewItem : ExtendedTreeViewItem
        <
            AssistantManagerTreeViewItem,
            Assistant,
            AssistantManagerTreeViewItemFilter
        >
    {
        internal string ModelName
        {
            get
            {
                if (_modelName == null)
                {
                    try
                    {
                        Model model = Data?.Model;
                        if (model == null)
                        {
                            _modelName = Data?.Model ?? "Unknown Model";
                        }
                        else
                        {
                            _modelName = model.Name ?? "Unknown Model";
                        }
                    }
                    catch
                    {
                        _modelName = "Unknown Model";
                    }
                }

                return _modelName;
            }
        }
        internal TextFormat ResponseFormat => _responseFormat ??= Data?.ResponseFormat.ToEnum<TextFormat>() ?? TextFormat.Auto;
        private TextFormat? _responseFormat;
        private string _modelName;

        public AssistantManagerTreeViewItem(int id, int depth, string displayName, Assistant data) : base(id, depth, displayName, data) { }

        public override int CompareTo(AssistantManagerTreeViewItem anotherItem, int columnIndex, bool ascending)
        {
            return columnIndex switch
            {
                AssistantManagerWindow.ColumnIndex.NAME => CompareByString(ascending, anotherItem, data => data.Data.Name),
                AssistantManagerWindow.ColumnIndex.MODEL => CompareByString(ascending, anotherItem, data => data.Data.Model),
                AssistantManagerWindow.ColumnIndex.CREATED_AT => CompareByUnixTime(ascending, anotherItem, data => data.Data.CreatedAt),
                AssistantManagerWindow.ColumnIndex.RESPONSE_FORMAT => CompareByString(ascending, anotherItem, data => data.Data.ResponseFormat),
                _ => 0
            };
        }

        public override bool Search(string searchString)
        {
            if (Data == null) return false;
            if (string.IsNullOrEmpty(searchString)) return true;
            if (Data.Id != null && Data.Id.Contains(searchString)) return true;
            if (Data.Name != null && Data.Name.Contains(searchString)) return true;
            if (Data.Model.Contains(searchString)) return true;
            if (Data.ResponseFormat.ToString().Contains(searchString)) return true;
            return false;
        }
    }
}