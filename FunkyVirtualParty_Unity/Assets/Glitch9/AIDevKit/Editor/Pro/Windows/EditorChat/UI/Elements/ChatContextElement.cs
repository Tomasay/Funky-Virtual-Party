using System;
using Glitch9.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal class ChatContextItem
    {
        internal string Id { get; }
        internal string Name { get; }
        internal Texture2D Icon { get; }
        internal Action<string> RemoveAction { get; }

        internal ChatContextItem(string id, string name, Texture2D icon, Action<string> remove = null)
        {
            Id = id;
            Name = name;
            Icon = icon;
            RemoveAction = remove;
        }
    }

    internal class ChatContextElement : VisualElement
    {
        internal ChatContextElement(ChatContextItem item)
        {
            AddToClassList(EditorChatConfig.ObjectItemClass);

            Image iconImg = new() { image = item.Icon };
            iconImg.AddToClassList(EditorChatConfig.ObjectIconClass);
            Add(iconImg);

            Label nameLabel = new(item.Name);
            nameLabel.AddToClassList(EditorChatConfig.ObjectNameClass);
            Add(nameLabel);

            if (item.RemoveAction != null)
            {
                Button removeButton = new() { style = { backgroundImage = EditorIcons.Delete as Texture2D } };
                removeButton.AddToClassList(EditorChatConfig.RemoveButtonClass);

                removeButton.clicked += () => item.RemoveAction(item.Id);
                Add(removeButton);
            }
        }
    }

    internal class ChatAttachedFileElement : VisualElement
    {
        internal ChatAttachedFileElement(ChatContextItem item)
        {
            AddToClassList(EditorChatConfig.AttachedFileClass);

            Image iconImg = new() { image = item.Icon };
            iconImg.AddToClassList(EditorChatConfig.AttachedFileIconClass);
            Add(iconImg);

            Label nameLabel = new(item.Name);
            nameLabel.AddToClassList(EditorChatConfig.AttachedFileNameClass);
            Add(nameLabel);

            if (item.RemoveAction != null)
            {
                Button removeButton = new() { style = { backgroundImage = EditorIcons.Delete as Texture2D } };
                removeButton.AddToClassList(EditorChatConfig.RemoveButtonClass);

                removeButton.clicked += () => item.RemoveAction(item.Id);
                Add(removeButton);
            }
        }
    }
}