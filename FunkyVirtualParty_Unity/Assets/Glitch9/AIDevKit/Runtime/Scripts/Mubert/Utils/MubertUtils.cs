using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Mubert
{
    internal class MubertUtils
    {
        internal static async UniTask<GeneratedAudio> ConvertToGeneratedAudio(List<TrackTask> trackTasks)
        {
            if (trackTasks == null) return null;

            List<AudioClip> clips = new();
            List<string> paths = new();

            foreach (var track in trackTasks)
            {
                string url = track.DownloadUrl;
                if (string.IsNullOrEmpty(url)) throw new System.Exception($"Failed to get download URL: {track.TaskStatusText}");
                AudioClip clip = await AudioClipLoader.LoadFullPathOrUrlAsync(url);
                if (clip == null) throw new System.Exception($"Failed to create audio clip from URL: {url}");
                clips.Add(clip);
                paths.Add(url);
            }

            if (clips.Count == 0) return null;

            return new GeneratedAudio(clips.ToArray(), paths.ToArray());
        }
    }
}