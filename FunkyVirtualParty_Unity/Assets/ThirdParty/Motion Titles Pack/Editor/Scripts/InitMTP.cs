using UnityEditor;

namespace Michsky.UI.MTP
{
    public class InitMUIP
    {
        [InitializeOnLoad]
        public class InitOnLoad
        {
            static InitOnLoad()
            {
                if (!EditorPrefs.HasKey("MTP.StyleCreator.Upgraded"))
                {
                    EditorPrefs.SetInt("MTP.StyleCreator.Upgraded", 1);
                    EditorPrefs.SetString("MTP.StyleCreator.RootFolder", "Motion Titles Pack/Style Creator/");
                }
            }
        }
    }
}