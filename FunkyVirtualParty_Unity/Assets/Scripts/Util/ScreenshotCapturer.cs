#if UNITY_EDITOR

using System.IO;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using NaughtyAttributes;

namespace UnityEngine.Recorder.Examples
{
    /// <summary>
    /// This example shows how to set up a recording session via script.
    /// To use this example, add the CaptureScreenShotExample component to a GameObject.
    ///
    /// Entering playmode will display a "Capture ScreenShot" button.
    ///
    /// Recorded images are saved in [Project Folder]/SampleRecordings
    /// </summary>
    public class ScreenshotCapturer : MonoBehaviour
    {
        [SerializeField]
        Vector2 resolution = new Vector2(3840, 2160);

        RecorderController m_RecorderController;

        void OnEnable()
        {
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            m_RecorderController = new RecorderController(controllerSettings);

            var mediaOutputFolder = Path.Combine(Application.dataPath, "..", "SampleRecordings");

            // Image
            var imageRecorder = ScriptableObject.CreateInstance<ImageRecorderSettings>();
            imageRecorder.name = "My Image Recorder";
            imageRecorder.Enabled = true;
            imageRecorder.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
            imageRecorder.CaptureAlpha = false;

            imageRecorder.OutputFile = Path.Combine(mediaOutputFolder, "image_") + DefaultWildcard.Take;

            imageRecorder.imageInputSettings = new GameViewInputSettings
            {
                OutputWidth = (int)resolution.x,
                OutputHeight = (int)resolution.y,
            };

            // Setup Recording
            controllerSettings.AddRecorderSettings(imageRecorder);
            controllerSettings.SetRecordModeToSingleFrame(0);
        }

        [Button]
        void CaptureScreenshot()
        {
            m_RecorderController.PrepareRecording();
            m_RecorderController.StartRecording();
        }
    }
}

#endif
