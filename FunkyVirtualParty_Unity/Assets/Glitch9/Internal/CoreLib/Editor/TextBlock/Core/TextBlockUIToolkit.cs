using System.Collections.Generic;
using Glitch9.Editor.UIToolkit;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor
{
    public interface IUIToolkitTextBlockRenderer
    {
        void Render(VisualElement root, TextBlock block, TextBlock prevBlock);
    }

    public class TextBlockUIToolkit
    {
        private static readonly Dictionary<BlockType, IUIToolkitTextBlockRenderer> _renderers = new()
        {
            { BlockType.Text, new UIToolkitPlainTextRenderer() },
            { BlockType.Header, new UIToolkitHeaderRenderer() },
            { BlockType.UList, new UIToolkitUListRenderer() },
            { BlockType.Quote, new UIToolkitQuoteRenderer() },
            { BlockType.CodeBlock, new UIToolkitCodeBlockRenderer() },
        };

        public static void Draw(VisualElement root, TextBlock block, TextBlock prevBlock)
        {
            if (block == null) return;

            if (_renderers.TryGetValue(block.type, out IUIToolkitTextBlockRenderer renderer))
            {
                renderer.Render(root, block, prevBlock);
            }
        }

        internal static void RenderText(VisualElement root, string text, string className)
        {
            if (string.IsNullOrEmpty(text)) return;
            root.Add(UIToolkitFactory.SelectableLabel(text, className));
        }
    }

    public class UIToolkitPlainTextRenderer : IUIToolkitTextBlockRenderer
    {
        public void Render(VisualElement root, TextBlock block, TextBlock prevBlock)
        {
            string text = block.content.Trim();
            TextBlockUIToolkit.RenderText(root, text, "text-block-plain-text");
        }
    }

    public class UIToolkitHeaderRenderer : IUIToolkitTextBlockRenderer
    {
        public void Render(VisualElement root, TextBlock block, TextBlock prevBlock)
        {
            string text = block.content.Trim();
            string className = $"text-block-header-level-{block.headerLevel}";  // get class name based on header level
            TextBlockUIToolkit.RenderText(root, text, className);
        }
    }

    public class UIToolkitUListRenderer : IUIToolkitTextBlockRenderer
    {
        public void Render(VisualElement root, TextBlock block, TextBlock prevBlock)
        {
            string text = block.content.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var uListElement = new VisualElement();
            uListElement.AddToClassList("text-block-ulist");

            var bulletElement = new Label("\u2022");
            bulletElement.AddToClassList("text-block-ulist-bullet");
            var contentElement = new Label(text);
            contentElement.AddToClassList("text-block-ulist-content");

            uListElement.Add(bulletElement);
            uListElement.Add(contentElement);

            root.Add(uListElement);
        }
    }

    public class UIToolkitQuoteRenderer : IUIToolkitTextBlockRenderer
    {
        public void Render(VisualElement root, TextBlock block, TextBlock prevBlock)
        {
            string text = block.content.Trim();
            TextBlockUIToolkit.RenderText(root, text, "text-block-quote");
        }
    }

    public class UIToolkitCodeBlockRenderer : IUIToolkitTextBlockRenderer
    {

        public void Render(VisualElement root, TextBlock block, TextBlock prevBlock)
        {
            string text = block.content.Trim();
            if (string.IsNullOrEmpty(text)) return;

            var container = new VisualElement();
            container.AddToClassList("text-block-code-block");

            var headerContainer = new VisualElement();
            headerContainer.AddToClassList("text-block-code-header");
            headerContainer.style.backgroundImage = EditorTextures.CodeBlockHeaderTexture;

            var languageLabel = new Label(block.language);
            languageLabel.AddToClassList("text-block-code-header-language");

            // Add save and copy buttons
            var headerButtonContainer = new VisualElement();
            headerButtonContainer.AddToClassList("text-block-code-header-buttons");

            var saveButton = new IconButton(
                icon: EditorIcons.Save as Texture2D,
                tooltip: "Save Code to File",
                onClick: () =>
                {
                    string directory = Application.dataPath;
                    string extension = TextBlockUtil.GetExtension(block.language);
                    string path = UnityEditor.EditorUtility.SaveFilePanel("Save Code", directory, "", extension);
                    if (!string.IsNullOrEmpty(path)) System.IO.File.WriteAllText(path, text);
                });

            saveButton.AddToClassList("text-block-code-header-button");

            var copyButton = new IconButton(
                icon: EditorIcons.Copy as Texture2D,
                tooltip: block.IsCopied ? "Copied!" : "Copy Code",
                onClick: () =>
                {
                    UnityEditor.EditorGUIUtility.systemCopyBuffer = text;
                    block.IsCopied = true;
                });

            copyButton.AddToClassList("text-block-code-header-button");

            string code;

            try
            {
                code = SyntaxHighlighter.Highlight(block.language, text);
            }
            catch
            {
                code = text;
            }

            var contentContainer = UIToolkitFactory.SelectableLabel(code);
            contentContainer.AddToClassList("text-block-code-content");
            contentContainer.style.backgroundImage = EditorTextures.CodeBlockBodyTexture;

            headerButtonContainer.Add(saveButton);
            headerButtonContainer.Add(copyButton);

            headerContainer.Add(languageLabel);
            headerContainer.Add(headerButtonContainer);

            container.Add(headerContainer);
            container.Add(contentContainer);

            root.Add(container);
        }
    }
}