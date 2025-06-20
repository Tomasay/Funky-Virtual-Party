using System;
using System.Reflection;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor
{
    public static class EditorAudioPlayer
    {
        private static bool _isPlaying = false;

        public static async void Play(AudioClip audioClip)
        {
            if (_isPlaying) Stop();
            Assembly unityAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "PlayPreviewClip",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] { typeof(AudioClip), typeof(Int32), typeof(Boolean) },
                null
            );
            method.Invoke(
                null,
                new object[] { audioClip, 0, false }
            );
            _isPlaying = true;
            // Delay for the length of the audio clip using UniTask
            await Task.Delay(TimeSpan.FromSeconds(audioClip.length));

            _isPlaying = false;
        }

        public static void Stop()
        {
            _isPlaying = false;
            Assembly unityAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityAssembly.GetType("UnityEditor.AudioUtil");
            MethodInfo method = audioUtilClass.GetMethod(
                "StopAllPreviewClips",
                BindingFlags.Static | BindingFlags.Public,
                null,
                new System.Type[] { },
                null
            );

            if (method != null)
            {
                method.Invoke(
                    null,
                    new object[] { }
                );
            }
        }

    }
}