using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public interface IIMGUITextBlockRenderer
    {
        void Render(TextBlock block, TextBlock prevBlock, float maxWidth);
    }

    public class TextBlockIMGUI
    {
        private static readonly Dictionary<BlockType, IIMGUITextBlockRenderer> _renderers = new()
        {
            { BlockType.Text, new IMGUIPlainTextRenderer() },
            { BlockType.Header, new IMGUIHeaderRenderer() },
            { BlockType.UList, new IMGUIUListRenderer() },
            { BlockType.Quote, new IMGUIQuoteRenderer() },
            { BlockType.CodeBlock, new IMGUICodeBlockRenderer() },
        };

        public static void Draw(TextBlock block, TextBlock prevBlock, float maxWidth)
        {
            if (block == null) return;

            if (_renderers.TryGetValue(block.type, out IIMGUITextBlockRenderer renderer))
            {
                renderer.Render(block, prevBlock, maxWidth);
            }
        }

        internal static void RenderText(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            if (string.IsNullOrEmpty(text)) return;
            GUILayout.TextArea(text, style, options);
        }
    }

    public class IMGUIPlainTextRenderer : IIMGUITextBlockRenderer
    {
        public void Render(TextBlock block, TextBlock prevBlock, float maxWidth)
        {
            string text = block.content.Trim();
            if (string.IsNullOrEmpty(text)) return;

            TextBlockIMGUI.RenderText(text, TextBlockIMGUIStyles.PlainText, GUILayout.MaxWidth(maxWidth));
        }
    }

    public class IMGUIHeaderRenderer : IIMGUITextBlockRenderer
    {
        public void Render(TextBlock block, TextBlock prevBlock, float maxWidth)
        {
            const int kMaxTopMargin = 12;
            const int kMaxBotMargin = 10;

            int headerLevel = block.headerLevel;
            int topMargin = Mathf.Clamp((headerLevel + 1) * 10, 2, kMaxTopMargin);
            int botMargin = Mathf.Clamp((headerLevel + 1) * 10, 2, kMaxBotMargin);

            GUIStyle headerStyle = new(TextBlockIMGUIStyles.PlainText)
            {
                fontSize = TextBlockIMGUIStyles.PlainText.fontSize + headerLevel,
                fontStyle = FontStyle.Bold,
                margin = new RectOffset(0, 0, topMargin, botMargin),
            };

            //ExGUILayout.SelectableLabel(block.content, maxWidth, headerStyle);
            //GUILayout.Label(block.content, headerStyle, GUILayout.MaxWidth(maxWidth));
            TextBlockIMGUI.RenderText(block.content, headerStyle, GUILayout.MaxWidth(maxWidth));
        }
    }

    public class IMGUIUListRenderer : IIMGUITextBlockRenderer
    {
        public void Render(TextBlock block, TextBlock prevBlock, float maxWidth)
        {
            string text = block.content.Trim();
            if (string.IsNullOrEmpty(text)) return;

            GUILayout.BeginHorizontal();
            GUILayout.Space(5f);
            GUILayout.Label("\u2022", TextBlockIMGUIStyles.UListDot);
            TextBlockIMGUI.RenderText(text, TextBlockIMGUIStyles.UList, GUILayout.MaxWidth(maxWidth));
            GUILayout.EndHorizontal();
        }
    }

    public class IMGUIQuoteRenderer : IIMGUITextBlockRenderer
    {
        public void Render(TextBlock block, TextBlock prevBlock, float maxWidth)
        {
            GUIStyle style = new(ExStyles.helpBox) { fontSize = 12, fontStyle = FontStyle.Italic };
            TextBlockIMGUI.RenderText(block.content, style, GUILayout.MaxWidth(maxWidth));
        }
    }

    public class IMGUICodeBlockRenderer : IIMGUITextBlockRenderer
    {
        public void Render(TextBlock block, TextBlock prevBlock, float maxWidth)
        {
            string text = block.content;
            GUILayout.BeginVertical(GUILayout.MaxWidth(maxWidth));
            try
            {
                GUILayout.Space(5f);

                DrawCodeBlockHeader(text, block, maxWidth);
                DrawCodeBlockBody(text, block, maxWidth);
            }
            finally
            {
                GUILayout.EndVertical();
            }
        }

        private void DrawCodeBlockHeader(string text, TextBlock block, float maxWidth)
        {
            GUILayout.BeginHorizontal(TextBlockIMGUIStyles.CodeBlockHeader, GUILayout.MaxWidth(maxWidth));
            try
            {
                GUILayout.Label(block.language, TextBlockIMGUIStyles.HeaderLabel);
                GUILayout.FlexibleSpace();

                // save button
                GUIContent saveBtnLabel = new(EditorIcons.Save, "Save Code");
                if (GUILayout.Button(saveBtnLabel, TextBlockIMGUIStyles.CodeBlockHeaderButton))
                {
                    string directory = Application.dataPath;
                    string extension = TextBlockUtil.GetExtension(block.language);
                    string path = EditorUtility.SaveFilePanel("Save Code", directory, "", extension);
                    if (!string.IsNullOrEmpty(path)) System.IO.File.WriteAllText(path, text);
                }

                GUILayout.Space(2f);

                GUIContent copyBtnLabel = new(EditorIcons.Copy, "Copy to Clipboard");//block.IsCopied ? new("\u2713 Copied!") : new("Copy Code");
                if (GUILayout.Button(copyBtnLabel, TextBlockIMGUIStyles.CodeBlockHeaderButton))
                {
                    EditorGUIUtility.systemCopyBuffer = text;
                    block.IsCopied = true;
                }

                GUILayout.Space(2f);
            }
            finally
            {
                GUILayout.EndHorizontal();
            }
        }

        private void DrawCodeBlockBody(string text, TextBlock block, float maxWidth)
        {
            string code;

            try
            {
                code = SyntaxHighlighter.Highlight(block.language, text);
            }
            catch
            {
                code = text;
            }

            TextBlockIMGUI.RenderText(code, TextBlockIMGUIStyles.CodeBlockContent, GUILayout.MaxWidth(maxWidth));
        }
    }
}
