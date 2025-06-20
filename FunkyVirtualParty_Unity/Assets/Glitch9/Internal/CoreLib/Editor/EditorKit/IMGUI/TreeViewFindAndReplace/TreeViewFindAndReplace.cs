using UnityEngine;

namespace Glitch9.Editor.IMGUI
{
    public abstract class TreeViewFindAndReplace
    {
        public void CaptureKeyboardInput()
        {
            Event e = Event.current;
            if (Event.current.type != EventType.KeyDown) return;
            if (!e.control) return;

            if (e.keyCode == KeyCode.R)
            {
                TreeViewFindAndReplaceWindow.Find(OnFindNext, OnReplaceNext, OnReplaceAll);
                e.Use();
            }
            else if (e.keyCode == KeyCode.F)
            {
                TreeViewFindAndReplaceWindow.Replace(OnFindNext, OnReplaceNext, OnReplaceAll);
                e.Use();
            }
        }

        protected abstract void OnFindNext(string findText);
        protected abstract void OnReplaceNext(string findText, string replaceText);
        protected abstract void OnReplaceAll(string findText, string replaceText);
    }
}