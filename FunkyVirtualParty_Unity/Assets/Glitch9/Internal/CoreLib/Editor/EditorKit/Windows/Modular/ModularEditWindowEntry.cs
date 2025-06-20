using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    internal interface IModularEditWindowEntry
    {
        void Draw();
        void Save();
        float MinHeight { get; }
        bool ExpandHeight { get; }
    }

    internal class ModularEditWindowTextField : ModularEditWindowEntry<string>
    {
        internal TextFieldType FieldType { get; set; } = TextFieldType.TextField;

        public override void Draw()
        {
            EditorGUILayout.BeginVertical(ExStyles.horizontalPaddedArea);
            {
                ExGUILayout.IconLabel(Label.text, Label.image, ExStyles.label);

                if (Description != null)
                {
                    EditorGUILayout.LabelField(Description, ExStyles.wordWrappedMiniLabel);
                }

                if (FieldType == TextFieldType.TextField)
                {
                    CurrentValue = EditorGUILayout.TextField(CurrentValue, ExStyles.textField, GetHeightOption());
                }
                else
                {
                    CurrentValue = EditorGUILayout.TextArea(CurrentValue, ExStyles.textField, GetHeightOption());
                }

                if (!Buttons.IsNullOrEmpty())
                {
                    GUILayout.BeginHorizontal();
                    {
                        foreach (var button in Buttons)
                        {
                            GUI.enabled = button.IsEnabled?.Invoke() ?? true;
                            if (GUILayout.Button(button.Label))
                            {
                                button.Callback?.Invoke();
                            }
                            GUI.enabled = true;
                        }
                    }
                    GUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    internal abstract class ModularEditWindowEntry<T> : IModularEditWindowEntry
    {
        internal GUIContent Label { get; set; }
        internal string Description { get; set; }
        internal T InitialValue
        {
            get => _initialValue;
            set
            {
                _initialValue = value;
                CurrentValue = value;
            }
        }
        internal T CurrentValue { get; set; }
        internal Action<T> OnSaveValue { get; set; }
        internal float? FixedHeight { get; set; }
        public bool ExpandHeight { get; set; } = false;
        internal List<ButtonEntry> Buttons { get; set; }
        internal T _initialValue;

        public abstract void Draw();

        public virtual void Save()
        {
            if (OnSaveValue != null && !Equals(CurrentValue, _initialValue))
            {
                OnSaveValue(CurrentValue);
            }
        }

        public float MinHeight
        {
            get
            {
                if (FixedHeight.HasValue)
                {
                    return FixedHeight.Value;
                }
                else if (ExpandHeight)
                {
                    return 100f;
                }
                else
                {
                    return 22f;
                }
            }
        }

        protected GUILayoutOption GetHeightOption()
        {
            if (FixedHeight.HasValue)
            {
                return GUILayout.Height(FixedHeight.Value);
            }
            else if (ExpandHeight)
            {
                return GUILayout.ExpandHeight(true);
            }
            else
            {
                return GUILayout.Height(22);
            }
        }

    }
}