
using UnityEngine;
using UnityEngine.UIElements;

namespace Glitch9.Editor.UIToolkit
{
    public class AudioPlayerElement : VisualElement
    {
        private readonly VisualElement _progressBarBackground;
        private readonly VisualElement _progressBarFill;
        private readonly Label _positionLabel;
        private readonly Button _playButton;
        private readonly Button _stopButton;

        private float _progress = 0f;
        private float _currentPosition = 0f;
        private float _clipLength;
        private bool _isPlaying = false;
        private readonly AudioClip _clip;

        private IVisualElementScheduledItem _scheduler;

        public AudioPlayerElement(AudioClip clip, float clipLength)
        {
            _clip = clip;
            _clipLength = clipLength;

            style.flexDirection = FlexDirection.Column;
            style.alignItems = Align.Center;
            style.paddingTop = 6;
            style.paddingBottom = 6;
            style.marginTop = 6;

            // _nameLabel = new Label(clipName)
            // {
            //     style =
            //     {
            //         unityTextAlign = TextAnchor.MiddleCenter,
            //         fontSize = 11,
            //         color = Color.cyan,
            //         width = Length.Percent(100),
            //         marginBottom = 4,
            //         whiteSpace = WhiteSpace.Normal // ← 줄바꿈 허용
            //     }
            // };
            // Add(_nameLabel);

            // Progress Bar: Background + Fill
            _progressBarBackground = new VisualElement
            {
                style =
                {
                    width = Length.Percent(100),
                    height = 10,
                    backgroundColor = new Color(0.2f, 0.2f, 0.2f),
                    marginTop = 4,
                    marginBottom = 4,
                    unityTextAlign = TextAnchor.MiddleCenter,
                }
            };
            _progressBarFill = new VisualElement
            {
                style =
                {
                    height = Length.Percent(100),
                    backgroundColor = ExColor.pink,
                }
            };
            _progressBarBackground.Add(_progressBarFill);
            Add(_progressBarBackground);

            // Current Position / Total Length
            _positionLabel = new Label($"{_currentPosition:F2} / {_clipLength:F2} sec")
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    color = EditorColors.gray,
                    width = Length.Percent(100),
                    marginBottom = 4
                }
            };
            Add(_positionLabel);

            // Button Row
            var buttonRow = new VisualElement
            {
                style = { flexDirection = FlexDirection.Row, justifyContent = Justify.Center, marginTop = 4 }
            };

            _playButton = new Button(() => Play()) { text = "▶", style = { width = 32, height = 20 } };
            _stopButton = new Button(() => Stop()) { text = "■", style = { width = 32, height = 20 } };

            buttonRow.Add(_playButton);
            buttonRow.Add(_stopButton);
            Add(buttonRow);

            // Initial state
            _progressBarFill.style.width = Length.Percent(0);
            _isPlaying = false;
        }

        private void Play()
        {
            if (_clip == null || _clipLength <= 0f) return;

            _isPlaying = true;
            _progress = 0f;
            EditorAudioPlayer.Play(_clip);

            float startTime = Time.realtimeSinceStartup;

            _scheduler?.Pause();
            _scheduler = schedule.Execute(() =>
            {
                _currentPosition = Time.realtimeSinceStartup - startTime;

                // update position label
                _positionLabel.text = $"{_currentPosition:F2} / {_clipLength:F2} sec";

                _progress = Mathf.Clamp01(_currentPosition / _clipLength);
                _progressBarFill.style.width = Length.Percent(_progress * 100);

                if (_progress >= 1.0f || !_isPlaying)
                {
                    _isPlaying = false;
                    _progress = 0f;
                    _currentPosition = 0f;
                    _positionLabel.text = $"0.00 / {_clipLength:F2} sec";
                    _progressBarFill.style.width = Length.Percent(0);
                    _scheduler.Pause();
                }
            }).Every(16);
        }

        private void Stop()
        {
            EditorAudioPlayer.Stop();
            _isPlaying = false;
            _progress = 0f;
            _progressBarFill.style.width = Length.Percent(0);
            _scheduler?.Pause();
        }
    }
}