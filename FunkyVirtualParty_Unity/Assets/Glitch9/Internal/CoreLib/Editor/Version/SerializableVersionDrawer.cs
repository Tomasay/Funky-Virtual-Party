using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    [CustomPropertyDrawer(typeof(SerializableVersion), true)]
    public class SerializableVersionDrawer : PropertyDrawer
    {
        private SerializedProperty build;
        private SerializedProperty releaseDate;

        private Rect labelRect;
        private Rect buildLabelRect;
        private Rect releaseDateLabelRect;

        private Rect majorRect;
        private Rect majorLeftBtnRect;
        // private Rect majorRightBtnRect;
        private Rect minorRect;
        private Rect minorLeftBtnRect;
        // private Rect minorRightBtnRect;
        private Rect patchRect;
        private Rect patchLeftBtnRect;
        private Rect undoBtnRect;

        private Rect buildRect;
        private Rect buildCopyButtonRect;
        private Rect releaseDateRect;

        private UnixTime _releaseDate;

        private bool _isInitialized = false;

        private int _major;
        private int _minor;
        private int _patch;

        private SerializableVersion GetTargetObject(SerializedProperty prop)
        {
            return fieldInfo.GetValue(prop.serializedObject.targetObject) as SerializableVersion;
        }

        private void Initialize(Rect position, SerializedProperty property)
        {
            build = property.FindPropertyRelative("build");
            releaseDate = property.FindPropertyRelative("releaseDate");

            Rect rect = position;
            rect.y -= EditorGUIUtility.singleLineHeight;

            // Layout (3 rows)
            // Label            [+]Major[-][+]Minor[-][+]Patch[-]
            // Build            [Build Number]
            // Release Date     [Release Date]

            float indentOffset = EditorGUI.indentLevel * 15f;

            Rect row1 = rect.GetSingleLightHeightRow(1);
            Rect row2 = rect.GetSingleLightHeightRow(2);
            Rect row3 = rect.GetSingleLightHeightRow(3);

            // Row1: Label, Major, Minor, Patch
            labelRect = row1.GetLabelRect();
            buildLabelRect = row2.GetLabelRect();
            releaseDateLabelRect = row3.GetLabelRect();

            Rect row1ValueRect = row1.GetValueRect();
            const float copyBtnWidth = 60;
            // buildRect = row2.GetValueRect();
            // buildRect.x -= indentOffset;

            Rect[] row2Split = row2.GetValueRect().SplitHorizontallyFixedReversed(copyBtnWidth);
            buildRect = row2Split[0];
            buildCopyButtonRect = row2Split[1];

            releaseDateRect = row3.GetValueRect();
            releaseDateRect.x -= indentOffset;

            const float kButtonWidth = 20;
            const int kFieldCount = 3;
            float sumBtnWidth = kButtonWidth * 2 * kFieldCount;

            float versionWidth = ((row1ValueRect.width - sumBtnWidth) / kFieldCount) + indentOffset;
            float currentX = row1ValueRect.x;

            // --- Major ---
            majorLeftBtnRect = new Rect(currentX, row1ValueRect.y, kButtonWidth, row1ValueRect.height);
            currentX += 5;

            majorRect = new Rect(currentX, row1ValueRect.y, versionWidth, row1ValueRect.height);
            currentX += versionWidth;

            //  majorRightBtnRect = new Rect(currentX, row1ValueRect.y, kButtonWidth, row1ValueRect.height);
            // currentX += kButtonWidth;

            // --- Minor ---
            minorLeftBtnRect = new Rect(currentX, row1ValueRect.y, kButtonWidth, row1ValueRect.height);
            currentX += 5;

            minorRect = new Rect(currentX, row1ValueRect.y, versionWidth, row1ValueRect.height);
            currentX += versionWidth;

            //minorRightBtnRect = new Rect(currentX, row1ValueRect.y, kButtonWidth, row1ValueRect.height);
            // currentX += kButtonWidth;

            // --- Patch ---
            patchLeftBtnRect = new Rect(currentX, row1ValueRect.y, kButtonWidth, row1ValueRect.height);
            currentX += 5;

            patchRect = new Rect(currentX, row1ValueRect.y, versionWidth, row1ValueRect.height);
            currentX += versionWidth;

            undoBtnRect = new Rect(currentX, row1ValueRect.y, kButtonWidth, row1ValueRect.height);



            if (_isInitialized) return;
            _isInitialized = true;

            _major = SerializableVersionUtil.GetMajor(build.intValue);
            _minor = SerializableVersionUtil.GetMinor(build.intValue);
            _patch = SerializableVersionUtil.GetPatch(build.intValue);
            _releaseDate = new UnixTime(releaseDate.longValue);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            Initialize(position, property);

            // Row 1
            EditorGUI.LabelField(labelRect, label);

            DrawVersionIncreaseButton(majorLeftBtnRect, VersionIncrement.Major, ref _major);
            EditorGUI.LabelField(majorRect, _major.ToString(), EditorStyles.textField);
            // DrawVersionDecreaseButton(majorRightBtnRect, ref _major);

            DrawVersionIncreaseButton(minorLeftBtnRect, VersionIncrement.Minor, ref _minor);
            EditorGUI.LabelField(minorRect, _minor.ToString(), EditorStyles.textField);
            // DrawVersionDecreaseButton(minorRightBtnRect, ref _minor);

            DrawVersionIncreaseButton(patchLeftBtnRect, VersionIncrement.Patch, ref _patch);
            EditorGUI.LabelField(patchRect, _patch.ToString(), EditorStyles.textField);
            // DrawVersionDecreaseButton(undoBtnRect, ref _patch);
            DrawUndoButton(undoBtnRect);

            // Row 2
            EditorGUI.LabelField(buildLabelRect, "Build");
            EditorGUI.LabelField(buildRect, build.intValue.ToString());
            if (GUI.Button(buildCopyButtonRect, "Copy"))
            {
                EditorGUIUtility.systemCopyBuffer = build.intValue.ToString();
            }

            // Row 3
            EditorGUI.LabelField(releaseDateLabelRect, "Release Date");
            EditorGUI.LabelField(releaseDateRect, _releaseDate.ToString("d"));

            EditorGUI.EndProperty();
        }

        private void DrawVersionIncreaseButton(Rect rect, VersionIncrement inc, ref int number)
        {
            if (GUI.Button(rect, "+"))
            {
                // number++;
                // build.intValue = SerializableVersion.CalcBuildNumber(_major, _minor, _patch);
                // releaseDate.longValue = UnixTime.Now;
                // _releaseDate = new UnixTime(releaseDate.longValue);
                number++;

                var target = GetTargetObject(build); // build 대신 property 넣어도 됨
                target.Increase(inc);              // 상태 저장
            }
        }

        private void DrawUndoButton(Rect rect)
        {
            if (GUI.Button(rect, "-"))
            {
                // number--;
                // // build.intValue = SerializableVersion.CalcBuildNumber(_major, _minor, _patch);
                // // releaseDate.longValue = UnixTime.Now;
                // // _releaseDate = new UnixTime(releaseDate.longValue);
                // UpdateVersion();
                var target = GetTargetObject(build);
                target.UndoIncrease();
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 2 * EditorGUIUtility.standardVerticalSpacing;
        }
    }
}