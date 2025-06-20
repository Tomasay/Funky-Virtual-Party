using Glitch9.Editor;
using Glitch9.Encryption;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    [CustomPropertyDrawer(typeof(ApiKey))]
    public class ApiKeyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            const float kBtnWidth = 20f;

            //var apiProp = property.FindPropertyRelative("api");
            var keyProp = property.FindPropertyRelative("key");
            var encryptProp = property.FindPropertyRelative("encrypt");
            var visibleProp = property.FindPropertyRelative("visible");

            float indent = EditorGUI.indentLevel * 15f + -2f;
            float labelWidth = EditorGUIUtility.labelWidth - indent;
            Rect labelRect = new(position.x, position.y, labelWidth, position.height);
            Rect rectLeftOver = new(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

            Rect[] rects;

            string labelText = label.text.Replace("Api", "API");


            if (encryptProp.boolValue)
            {
                labelText += " (encrypted)";
                rects = rectLeftOver.SplitHorizontallyFixedReversed(kBtnWidth);
            }
            else
            {
                rects = rectLeftOver.SplitHorizontallyFixedReversed(kBtnWidth, kBtnWidth);
            }


            Rect keyRect = rects[0];

            EditorGUI.LabelField(labelRect, labelText);

            if (encryptProp.boolValue)
            {
                const string encryptMessage = "Using an encrypted API key";
                ExGUI.BoxedLabel(keyRect, encryptMessage);
                Rect btnRect = rects[1];
                DrawDeleteButton(btnRect);
            }
            else
            {
                if (visibleProp.boolValue)
                {
                    EditorGUI.PropertyField(keyRect, keyProp, GUIContent.none);
                }
                else
                {
                    string newApiKey = EditorGUI.PasswordField(keyRect, GUIContent.none, keyProp.stringValue);
                    if (newApiKey != keyProp.stringValue) keyProp.stringValue = newApiKey;
                }

                Rect btnRect1 = rects[1];
                Rect btnRect2 = rects[2];
                DrawHideButton(btnRect1);
                DrawEncryptButton(btnRect2);
            }

            EditorGUI.EndProperty();

            void DrawHideButton(Rect rect)
            {
                bool newVisible = GUI.Toggle(rect, visibleProp.boolValue, EditorIcons.Hide, EditorStyles.miniButtonMid);
                if (newVisible != visibleProp.boolValue) visibleProp.boolValue = newVisible;
            }

            void DrawEncryptButton(Rect rect)
            {
                if (GUI.Button(rect, EditorIcons.Key, EditorStyles.miniButtonRight))
                {
                    if (string.IsNullOrEmpty(keyProp.stringValue))
                    {
                        EditorUtility.DisplayDialog("Error", "API key is empty. Please enter a valid API key.", "OK");
                        return;
                    }

                    if (ShowDialog.Confirm("Are you sure you want to encrypt the API key? You won't be able to decrypt it for safty reasons."))
                    {
                        encryptProp.boolValue = true;
                        keyProp.stringValue = Encrypter.EncryptString(keyProp.stringValue);
                        EditorUtility.DisplayDialog("Success", "API Key encrypted successfully!", "OK");
                    }
                }
            }

            void DrawDeleteButton(Rect rect)
            {
                if (GUI.Button(rect, EditorIcons.Delete, EditorStyles.miniButtonRight))
                {
                    if (EditorUtility.DisplayDialog("Delete Encrypted API Key", "Are you sure you want to delete the encrypted API Key?", "Yes", "No"))
                    {
                        keyProp.stringValue = string.Empty;
                        encryptProp.boolValue = false;
                        visibleProp.boolValue = true; // Reset visibility to true when deleting
                        property.serializedObject.ApplyModifiedProperties();
                        EditorUtility.SetDirty(property.serializedObject.targetObject);
                    }
                }
            }
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}