using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.AIDevKit.Editor.UnityAssistant
{
    internal enum RequestType
    {
        LLM,
        TTS,
        // add other types as needed
    }

    internal class ProgressBarController
    {
        //private const string kHiddenClass = "progress-bar-container--hidden"; // Class to hide the progress bar container
        private const int kProgressBarUpdateInterval = 50; // Interval in milliseconds to update the progress bar
        private readonly EditorChatWindow _window;
        private readonly VisualElement _progressBarContainer;
        private readonly ProgressBar _progressBar;
        private bool _isProgressBarVisible = false;
        private IVisualElementScheduledItem _progressBarUpdateTask; // Task to update the progress bar 
        private DateTime _startTime; // Start time for the progress bar

        internal ProgressBarController(EditorChatWindow window)
        {
            _window = window;
            var root = _window.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("Root VisualElement is null. ProgressBarController cannot be initialized.");
                return;
            }

            _progressBarContainer = root.Q<VisualElement>("progress-bar-container");

            if (_progressBarContainer == null)
            {
                int childCount = root.contentContainer.Children().Count();
                Debug.LogError($"ProgressBarContainer not found. Child count: {childCount}. Ensure the UXML is correctly set up.");
                return;
            }

            _progressBar = _progressBarContainer.Q<ProgressBar>("progress-bar");
        }


        internal void ShowRequestProgressBar(RequestType type, string modelId)
        {
            if (string.IsNullOrEmpty(modelId)) modelId = "unknown_model";
            //string resName = ResolveResName(type); 

            //int timeout = AIDevKitSettings.RequestTimeout;
            int timeout = 6;

            ShowProgressBar();
            _startTime = DateTime.Now; // Record the start time for the progress bar

            // start a repeating task to update the progress bar 
            _progressBarUpdateTask = _progressBar.schedule.Execute(() =>
            {
                if (_progressBar == null) return;

                // Update the progress bar value to simulate loading
                float progress = Mathf.Clamp01((float)(DateTime.Now - _startTime).TotalSeconds / timeout); // Calculate progress based on elapsed time

                if (progress >= 1f)
                {
                    _progressBarUpdateTask?.Pause(); // Pause the task when progress reaches 100%
                    _progressBarUpdateTask = null; // Clear the task reference
                }
                UpdateProgressBar(progress);
            }).Every(kProgressBarUpdateInterval); // Update every 200 milliseconds 
        }

        private void ShowProgressBar()
        {
            if (_progressBar == null) return;

            // _progressBarContainer.style.display = DisplayStyle.Flex; // Show the progress bar
            // _progressBarContainer.RemoveFromClassList(kHiddenClass); // Show the progress bar by removing the hidden class
            _progressBarContainer.style.opacity = 1f; // Set opacity to 1 for visibility
            _progressBar.value = 0f; // Reset progress bar value 

            _isProgressBarVisible = true; // Mark progress bar as visible
        }

        private void UpdateProgressBar(float progress)
        {
            if (_progressBar == null) return;

            // Ensure progress is clamped between 0 and 1 
            _progressBar.lowValue = 0;
            _progressBar.highValue = 1;
            _progressBar.value = progress; // Update the progress bar value 
            _progressBar.MarkDirtyRepaint();
        }

        internal void HideProgressBar()
        {
            if (_progressBar == null) return;

            //AIDevKitDebug.Mark("HideProgressBar called");

            _progressBarUpdateTask?.Pause(); // Pause the task to stop updates 
            _progressBarUpdateTask = null; // Clear the task reference

            _progressBar.value = 0f; // Reset progress bar value 
            _progressBarContainer.style.opacity = 0f; // Optionally set opacity to 0 for a fade-out effect
            //_progressBarContainer.style.display = DisplayStyle.None; // Hide the progress bar  

            // 숨기기 
            _isProgressBarVisible = false; // Mark progress bar as hidden
        }

        internal void ToggleProgressBar()
        {
            //AIDevKitDebug.Mark("ToggleProgressBar called");
            if (_progressBarContainer == null) return;

            _isProgressBarVisible = !_isProgressBarVisible; // Toggle visibility state
            //_progressBarContainer.style.display = _isProgressBarVisible ? DisplayStyle.Flex : DisplayStyle.None; // Show or hide the progress bar 
        }
    }
}