using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    /// <summary>
    /// Flutter style showDialog service.
    /// </summary>
    public static partial class ShowDialog
    {
        private const string kMessageTitle = "Info";
        private const string kConfirmationTitle = "Confirmation";
        private const string kWarningTitle = "Warning";
        private const string kErrorTitle = "Error";
        private const string kOkLabel = "Ok";
        private const string kCancelLabel = "Cancel";
        private const float kExtendedMessageWidth = 400f;
        private const float kExtendedMessageHeight = 600f;

        private static string Merge(string[] messages)
            => string.Join(Environment.NewLine, messages.Select(m => m.Trim()).Where(m => !string.IsNullOrEmpty(m)));

        public static bool Info(params string[] messages) => EditorUtility.DisplayDialog(kMessageTitle, Merge(messages), kOkLabel);
        public static bool Confirm(params string[] messages) => EditorUtility.DisplayDialog(kConfirmationTitle, Merge(messages), kOkLabel, kCancelLabel);
        public static bool Warning(params string[] messages) => EditorUtility.DisplayDialog(kWarningTitle, Merge(messages), kOkLabel);
        public static bool Error(params string[] messages) => EditorUtility.DisplayDialog(kErrorTitle, Merge(messages), kOkLabel);
        public static bool Error(Exception exception) => Error(exception.Message);


        public static void ExtendedMessage(string title, string message, Vector2? size = null)
        {
            size ??= new Vector2(kExtendedMessageWidth, kExtendedMessageHeight);
            ExtendedMessageWindow.Show(title, message, kOkLabel, size.Value);
        }
    }
}
