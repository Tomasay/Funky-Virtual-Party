using System.Collections.Generic;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Generation
{
    internal class CodeGenPromptGuideWindow : PaddedEditorWindow
    {
        internal static void ShowWindow()
        {
            var window = GetWindow<CodeGenPromptGuideWindow>(true, "How to Write Effective Prompts", true);
            window.Show();
        }

        private List<TextBlock> _headerBlocks;
        private List<TextBlock> _tipsBlocks;
        private List<TextBlock> _checklistBlocks;
        private Vector2 _scrollPos;

        private void OnEnable()
        {
            _headerBlocks ??= new()
            {
                TextBlock.Header("Prompting for Better Code", headerLevel: 2),
                TextBlock.Text("To get more accurate and usable results from the AI, " +
                    "it's important to know how to clearly communicate your intent. " +
                    "The better your prompt, the more reliable and tailored the generated code will be."),
                TextBlock.Text("Here’s a condensed guide, including best practices and tips to help you write better prompts for Unity-related scripting tasks."),
            };

            _tipsBlocks ??= new()
            {
                TextBlock.Header("1. Be Specific", headerLevel: 1),
                TextBlock.UList("Clearly describe what the script should do. Mention expected inputs, outputs, and any specific Unity components, APIs, or frameworks you want to use. Keep it focused—enough detail to be clear, but not so much that it becomes confusing or vague."),
                TextBlock.Header("  Bad"),
                TextBlock.UList("I want a script for a game where you play as a wizard."),
                TextBlock.Header("  Good"),
                TextBlock.UList("A character movement controller in a 3D RPG. The character can move and jump using player input. Include parameters to control movement speed and jump height."),
                TextBlock.Header("2. Use Proper Grammar and Punctuation"),
                TextBlock.UList("Well-written prompts produce better results. Always capitalize the first letter of each sentence and end with proper punctuation. Avoid typos and run-on sentences."),
                TextBlock.Header("  Bad"),
                TextBlock.UList("Script that moves a game object from left to right over 5 seconds on the x axis and the object is a cube."),
                TextBlock.Header("  Good"),
                TextBlock.UList("Script that smoothly moves a GameObject, represented by a cube, from left to right over a 5-second duration, along the X-axis."),
                TextBlock.Header("3. Limit the Scope", headerLevel: 1),
                TextBlock.UList("Break down complex behavior into smaller, isolated prompts. Requesting too many features at once often leads to vague or incomplete results. Keep your requests focused on a single responsibility per script."),
                TextBlock.Header("  Bad"),
                TextBlock.UList("A character that can move, jump, attack, take damage, and pick up items."),
                TextBlock.Header("  Good"),
                TextBlock.UList("A 3D character movement controller that supports jumping using Rigidbody."),
                TextBlock.UList("A combat system that handles melee attacks with adjustable damage."),
                TextBlock.UList("A health manager that tracks damage and triggers death effects."),
                TextBlock.UList("An item pickup script that detects collisions with collectible items."),
                TextBlock.Header("4. Be Realistic", headerLevel: 1),
                TextBlock.UList("Understand that AI is great at generating foundational code, but not full production-level systems out of the box. Use it to scaffold your systems and then build on top of that."),
                TextBlock.Header("  Bad"),
                TextBlock.UList("I want a script that generates a full city with roads, traffic, and pedestrians."),
                TextBlock.Header("  Good"),
                TextBlock.UList("Script that generates a grid-based city layout with randomly placed building prefabs of various heights."),
            };

            _checklistBlocks ??= new()
            {
                TextBlock.Header("Quick Checklist", headerLevel: 1),
                TextBlock.UList("Is the functionality clearly described?"),
                TextBlock.UList("Are grammar and punctuation correct?"),
                TextBlock.UList("Is the prompt concise but informative?"),
                TextBlock.UList("Is the task small enough to be handled in one script?"),
                TextBlock.UList("Is the prompt realistically achievable by the AI?"),
            };
        }

        protected override void DrawGUI()
        {
            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, false, true);
            {
                float width = position.width - 20;
                TextBlock.DrawIMGUI(_headerBlocks, width);
                EditorGUILayout.Space(10);
                TextBlock.DrawIMGUI(_tipsBlocks, width);
                EditorGUILayout.Space(10);
                TextBlock.DrawIMGUI(_checklistBlocks, width);
            }
            EditorGUILayout.EndScrollView();
        }
    }
}