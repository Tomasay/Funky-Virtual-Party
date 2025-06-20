using System;
using System.Collections.Generic;
using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal static class EditorChatConfig
    {
        // UI Toolkit Class Names -----------------------------------------------------
        internal const string ObjectItemClass = "object-item";
        internal const string ObjectIconClass = "object-icon";
        internal const string ObjectNameClass = "object-name";

        internal const string AttachedFileClass = "chat-message-attached-file";
        internal const string AttachedFileIconClass = "chat-message-attached-file-icon";
        internal const string AttachedFileNameClass = "chat-message-attached-file-name";

        internal const string RemoveButtonClass = "remove-button";
        internal static readonly Color InputFieldBorderColor = new(0.32f, 0.24f, 1.00f);//new(0.35f, 0.25f, 1f); // RGB to Color
        internal static readonly Color ProgressBarPinkFill = new(1.00f, 0.24f, 0.32f); // RGB to Color

        // Fixed Values ------------------------------------------------------------- 
        internal const int kMinTokens = 500;
        internal const int kMaxTokens = 10000;
        internal const int kTypingIndicatorMax = 8;
        internal const double kTypingIndicatorUpdateInterval = 0.5f;
        internal const string kSenderName = AIDevKitEditor.Labels.EditorChat;
        internal const int DefaultDisplayedMessageCount = 50;
        internal const int ShowMoreMessagesIncrement = 25;

        // Chatbot Info ------------------------------------------------------------- 
        internal const string kChatbotVer = "v5";
        internal const string kChatbotId = "editor_chatbot_" + kChatbotVer;
        internal const string kChatbotName = "Editor Chatbot " + kChatbotVer;
        internal const string kChatbotDescription = "A chatbot within the Unity Editor. It can help you with any questions you have about Unity development.";
        //internal const string kChatbotInstruction = "Please answer the question as if you were a Unity developer. You can also provide code snippets and examples to help the user understand better.";
        /*
            Scene: SampleScene
            Active Selection: GameObject(name="Enemy", components=[Transform, Rigidbody, Collider])
            User prompt: "이 오브젝트가 왜 안 움직일까?"
        */
        internal const string kChatbotInstruction = @"
You are a Unity Editor Assistant integrated inside the Unity Editor.

Your role is to help the user understand, debug, or modify their Unity project.  
The user will provide natural language prompts along with editor context, such as the current scene name and the selected GameObject in the following format:

---
Scene: SampleScene  
Active Selection: GameObject(name=""Enemy"", components=[Transform, Rigidbody, Collider])  
User Prompt: ""Why is this object not moving?""
Console Errors:
- ""NullReferenceException: Object reference not set to an instance of an object""
- ""ArgumentException: The object is not active in the hierarchy""
---

Use the scene and selection context to provide intelligent, context-aware feedback.  
Assume the user is working inside Unity Editor, and your answer should reflect how a Unity developer would think.

### Guidelines:
- If the GameObject contains certain components (e.g. Rigidbody), suggest checking their properties (e.g. isKinematic)
- If expected components are missing, suggest adding them
- Provide Unity-specific solutions, not generic programming answers
- Use concise explanations, but include helpful reasoning
- Never refer to yourself as an AI language model
- Do not repeat the context back to the user 
";

        internal const string kChatbotStartingMessage = "Hello! I'm here to help you with <b>Unity development</b>. What can I do for you today?";
        internal const string kChatbotFailResponse = "I'm sorry, There was an error processing your request.";

        // UI ---------------------------------------------------------------------  
        internal const float kThreadSectionFlex = 0.25f;
        internal const float kSettingsLabelWidth = 180f;
        internal const float kInputFieldMinHeight = 38f;
        internal const float kInputFieldMaxHeight = 200f;
        internal const int kInputFieldSendBtnPadding = 10;
        internal const float kInputFieldSendBtnWidth = 38f;
        internal const int kInputFieldMargin = 4;
        internal const int kInputFieldBorder = 2;
        internal const float kThreadListRemoveBtnWidth = 20f;
        internal const float kChatListWidthOffset = 18f;
        internal const float kChatMinHeight = 70f;
        internal const int kChatPadding = 10;
        internal const float kAttachedImageWidth = 72f;
        internal const float kAttachedImageHeight = 72f;
        internal const float kAttachedScriptWidth = 120f;
        internal const float kAttachedScriptHeight = 60f;

        // Window Titles ---------------------------------------------------------
        internal const string SettingsWindowTitle = AIDevKitEditor.Labels.EditorChat + " Settings";

        // UI Labels --------------------------------------------------------- 
        internal static readonly GUIContent kToolbarSaveAsTextLabel = new(EditorIcons.Import, "Export conversation as a text file");
        internal static readonly GUIContent kToolbarSettingsLabel = new(EditorIcons.Settings, "Open settings window");

        // Attachment Extensions ---------------------------------------------------------
        internal static readonly HashSet<string> ImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".gif"
        };

        internal static readonly HashSet<string> TextExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".cs", ".txt", ".json", ".xml", ".md", ".html", ".css", ".js", ".ts",
            ".shader", ".shadertoy", ".shadergraph", ".compute", ".txtasset",
            ".uss", ".uxml", ".jsonc", ".yaml", ".yml", ".csv", ".tsv", ".log",
            ".config", ".ini", ".asset",
        };

        internal static readonly HashSet<string> AudioExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".wav", ".mp3", ".ogg", ".aiff", ".flac"
        };
    }
}