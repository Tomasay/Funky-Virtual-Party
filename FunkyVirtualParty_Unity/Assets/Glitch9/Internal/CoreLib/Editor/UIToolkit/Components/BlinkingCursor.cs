
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    internal class BlinkingCursor : Manipulator
    {
        private const string kStyleSheetFileName = "BlinkingCursor";
        private IVisualElementScheduledItem m_ScheduledBlink;

        private int m_Interval = 500;
        private StyleSheet m_StyleSheet;
        private bool blinkOn = true;
        private TextField textField => target as TextField;

        public int interval
        {
            get => m_Interval;
            set
            {
                m_Interval = value;
                m_ScheduledBlink?.Pause();
                m_ScheduledBlink = target?.schedule.Execute(UpdateCursorColor).Every(interval);
                m_ScheduledBlink?.Pause();
            }
        }

        protected override void RegisterCallbacksOnTarget()
        {
            if (target is TextField tf)
            {
                // Load USS once when first needed
                if (m_StyleSheet == null)
                    m_StyleSheet = USSStyles.Get(kStyleSheetFileName);

                if (m_StyleSheet == null)
                {
                    Debug.LogWarning($"BlinkingCursor: StyleSheet '{kStyleSheetFileName}' not found. Ensure it is in the project.");
                }

                if (m_StyleSheet != null && !tf.styleSheets.Contains(m_StyleSheet))
                    tf.styleSheets.Add(m_StyleSheet);

                tf.AddToClassList("text-field-cursor-visible");

                tf.RegisterCallback<FocusInEvent>(OnFocusIn);
                tf.RegisterCallback<FocusOutEvent>(OnFocusOut);
                interval = m_Interval;
            }
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            if (target is TextField tf)
            {
                tf.UnregisterCallback<FocusInEvent>(OnFocusIn);
                tf.UnregisterCallback<FocusOutEvent>(OnFocusOut);
                m_ScheduledBlink?.Pause();
                m_ScheduledBlink = null;
            }
        }

        private void OnFocusIn(FocusInEvent e) => m_ScheduledBlink?.Resume();
        private void OnFocusOut(FocusOutEvent e) => m_ScheduledBlink?.Pause();

        private void UpdateCursorColor()
        {
            if (textField == null) return;

            blinkOn = !blinkOn;

            if (blinkOn)
            {
                textField.RemoveFromClassList("text-field-cursor-hidden");
                textField.AddToClassList("text-field-cursor-visible");
            }
            else
            {
                textField.RemoveFromClassList("text-field-cursor-visible");
                textField.AddToClassList("text-field-cursor-hidden");
            }
        }
    }
}
