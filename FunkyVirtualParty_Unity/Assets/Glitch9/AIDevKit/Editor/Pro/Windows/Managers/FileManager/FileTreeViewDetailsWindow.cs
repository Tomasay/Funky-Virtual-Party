using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using UnityEngine;
using Glitch9.IO.Networking.RESTApi;

namespace Glitch9.AIDevKit.Editor.Files
{
    public partial class FileManagerWindow
    {
        public class FileTreeViewDetailsWindow : ExtendedTreeViewDetailsWindow
        {
            private string DateString
            {
                get
                {
                    if (string.IsNullOrEmpty(dateString))
                    {
                        dateString = Data?.CreatedAt == null ? "Unknown" : Data.CreatedAt.Value.ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    return dateString;
                }
            }

            private string dateString;

            protected override void DrawSubtitle()
            {
                TreeViewGUI.SubtitleLeft($"Created At: {DateString}");
            }

            protected override void DrawBody()
            {
                TreeViewGUI.BeginSection("File Details");
                {
                    AIDevKitGUI.CopiableLabelField("API", Item.Api.ToApiValue());
                    AIDevKitGUI.CopiableLabelField("File ID", Item.FileId);
                    AIDevKitGUI.CopiableLabelField("File URI", Item.Uri);
                    AIDevKitGUI.CopiableLabelField("File Name", Item.FileName);
                    AIDevKitGUI.CopiableLabelField("File Size", Item.FormattedSize);
                    AIDevKitGUI.CopiableLabelField("MIME Type", Item.MimeType.ToApiValue());
                    AIDevKitGUI.CopiableLabelField("Created At", Item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"));
                    AIDevKitGUI.CopiableLabelField("Expires At", Item.ExpiresAt.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                TreeViewGUI.EndSection();

                TreeViewGUI.BeginSection("Metadata");
                {
                    if (Data.Metadata.IsNotNullOrEmpty())
                    {
                        foreach (var kvp in Data.Metadata)
                        {
                            AIDevKitGUI.CopiableLabelField(kvp.Key, kvp.Value);
                        }
                    }
                    else
                    {
                        AIDevKitGUI.CopiableLabelField("No Metadata", "No metadata available for this file.");
                    }
                }
                TreeViewGUI.EndSection();
            }
        }
    }
}