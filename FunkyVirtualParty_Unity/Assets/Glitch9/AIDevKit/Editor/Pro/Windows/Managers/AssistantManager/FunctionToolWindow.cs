using Glitch9.Editor;
using Glitch9.Editor.IMGUI;
using Glitch9.IO.Json.Schema;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor.Assistants
{
    public class FunctionToolWindow : PaddedEditorWindow
    {
        private const float TEXT_FIELD_HEIGHT = 80f;
        private const float DEFAULT_LINE_HEIGHT_MULTIPLIER = 2.4f;
        private const float ENUM_LINE_HEIGHT_MULTIPLIER = 3.6f;

        public FunctionDeclaration Function { get; set; }
        public FunctionDeclaration Modified { get; set; }

        private FuncParam[] _funcParams;
        private ReorderableList _functionParamsReorderableList;

        public static void ShowWindow(FunctionDeclaration function)
        {
            FunctionToolWindow toolWindow = GetWindow<FunctionToolWindow>();
            toolWindow.titleContent = new GUIContent(function.Name);

            function.Parameters ??= new();
            function.Parameters.Properties ??= new();
            function.Parameters.Required ??= new();

            toolWindow.Function = function;
            toolWindow.Modified = function;

            List<FuncParam> funcParams = new();

            foreach (KeyValuePair<string, JsonSchema> kvp in function.Parameters.Properties)
            {
                string propertyName = kvp.Key;
                JsonSchema jsonSchema = kvp.Value;
                bool required = false;

                foreach (string reqPropertyName in function.Parameters.Required)
                {
                    if (reqPropertyName == propertyName)
                    {
                        required = true;
                    }
                }

                funcParams.Add(new FuncParam(propertyName, jsonSchema, required));
            }

            toolWindow._funcParams = funcParams.ToArray();
            toolWindow.Show();
        }

        protected override void DrawGUI()
        {
            if (Function == null)
            {
                EditorGUILayout.LabelField("Function is null.");
                return;
            }


            GUILayout.Label(Function.Name, TreeViewStyles.DetailsWindowTitle);

            GUILayout.BeginVertical(TreeViewStyles.EditWindowBody);
            {
                // edit name
                Modified.Name = EditorGUILayout.TextField("Function Name", Modified.Name);

                // edit description
                EditorGUILayout.LabelField("Function Description");
                Modified.Description = EditorGUILayout.TextArea(Modified.Description, TreeViewStyles.WordWrapTextField, GUILayout.Height(TEXT_FIELD_HEIGHT));

                GUILayout.Space(10);

                DrawFunctionProperties();
            }
            GUILayout.EndVertical();
        }

        private void DrawFunctionProperties()
        {
            _functionParamsReorderableList ??= ExGUIUtility.CreateReorderableList(_funcParams, FuncParamsDrawer, new GUIContent("Function Parameters"), GetHeightMultiplier);
            _functionParamsReorderableList.DoLayoutList();
        }

        private float GetHeightMultiplier(int toolIndex)
        {
            if (_funcParams.Length == 0 || toolIndex >= _funcParams.Length)
            {
                return DEFAULT_LINE_HEIGHT_MULTIPLIER;
            }

            return _funcParams[toolIndex].type == JsonSchemaType.Enum ? ENUM_LINE_HEIGHT_MULTIPLIER : DEFAULT_LINE_HEIGHT_MULTIPLIER;
        }

        private FuncParam FuncParamsDrawer(Rect rect, int index, FuncParam param)
        {
            param ??= new();

            bool isEnum = param.type == JsonSchemaType.Enum;
            int rowCount = isEnum ? 3 : 2;
            Rect[] rows = rect.SplitVertically(rowCount, 2);
            Rect[] firstRowColumns = rows[0].SplitHorizontally(2, .42f, .42f, .12f, .04f);

            param.type = (JsonSchemaType)EditorGUI.EnumPopup(firstRowColumns[0], param.type);
            param.propertyName = EditorGUI.TextField(firstRowColumns[1], param.propertyName);
            EditorGUI.LabelField(firstRowColumns[2], "Required", TreeViewStyles.ChildWindowSubtitleRight);
            param.required = EditorGUI.Toggle(firstRowColumns[3], param.required);
            param.description = EditorGUI.TextField(rows[1], param.description);
            if (isEnum) param.enumValues = DrawEnumValues(rows[2], param.enumValues);

            return param;
        }

        private List<string> DrawEnumValues(Rect rect, List<string> enumValues)
        {
            const float LAST_ADD_BTN_WIDTH = 20f;
            const int SPACE = 2;

            int count = enumValues.Count;
            Rect[] rects = new Rect[count];
            float rowWidth = rect.width - LAST_ADD_BTN_WIDTH;
            float width = rowWidth / count;

            for (int i = 0; i < count; i++)
            {
                rects[i] = new Rect(rect.x + width * i + SPACE * i, rect.y, width - SPACE, rect.height);
            }

            for (int i = 0; i < count; i++)
            {
                enumValues[i] = EditorGUI.TextField(rects[i], enumValues[i]);
            }

            Rect addBtnRect = new Rect(rect.x + rowWidth + SPACE, rect.y, LAST_ADD_BTN_WIDTH, rect.height);

            if (GUI.Button(addBtnRect, "+"))
            {
                enumValues.Add("");
            }

            return enumValues;
        }

        private class FuncParam
        {
            internal JsonSchemaType type;
            internal string propertyName;
            internal string description; // 1 line
            internal List<string> enumValues;
            internal bool required;

            internal FuncParam()
            {
            }

            internal FuncParam(string propertyName, JsonSchema jsonSchema, bool required)
            {
                this.propertyName = propertyName;
                type = jsonSchema.Type;
                description = jsonSchema.Description;
                enumValues = jsonSchema.Enum;
                this.required = required;
            }
        }
    }
}