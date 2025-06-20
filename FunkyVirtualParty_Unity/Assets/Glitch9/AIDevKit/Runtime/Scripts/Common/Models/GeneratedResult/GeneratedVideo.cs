using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using Glitch9.IO.Networking;
using UnityEngine;
using UnityEngine.Video;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Unity does not support creation of a <see cref="VideoClip"/> on runtime.
    /// Therefore, <see cref="GeneratedVideo"/> will only save the urls of the video files.
    /// </summary>
    public class GeneratedVideo : GeneratedFile<string, RawFile>
    {
        public static implicit operator string(GeneratedVideo generatedVideo) => generatedVideo?.values?.FirstOrDefault();
        public static implicit operator string[](GeneratedVideo generatedVideo) => generatedVideo?.values;
        public static implicit operator RawFile(GeneratedVideo generatedVideo) => generatedVideo?.ToFiles()?.FirstOrDefault();
        public static implicit operator RawFile[](GeneratedVideo generatedVideo) => generatedVideo?.ToFiles();

        public GeneratedVideo(string url, string path, Usage usage) : base(url, path, usage) { }
        public GeneratedVideo(string[] urls, string[] paths, Usage usage) : base(urls, paths, usage) { }

        public override RawFile[] ToFiles()
        {
            List<RawFile> files = new();

            for (int i = 0; i < values.Length; i++)
            {
                string path = null;
                string url = null;

                if (i < Paths.Length) path = Paths[i];
                if (i < values.Length) url = values[i];

                if (url == null)
                {
                    Debug.LogWarning($"URL at index {i} is null. Skipping file creation.");
                    continue;
                }

                files.Add(new RawFile(filePath: path, url: url, note: fileNote));
            }

            return files.ToArray();
        }
    }

    internal static class GeneratedVideoFactory
    {
        public static async UniTask<GeneratedVideo> CreateAsync(IList<string> urls, string outputPath, Model model, MIMEType mimeType, Usage usage = null)
        {
            //if (string.IsNullOrEmpty(outputPath)) throw new ArgumentNullException(nameof(outputPath), "Output path cannot be null or empty.");
            if (urls == null || urls.Count == 0) throw new ArgumentNullException(nameof(urls), "URLs cannot be null or empty.");
            if (model == null) throw new ArgumentNullException(nameof(model), "Model cannot be null.");

            List<string> resolvedDlPaths = new();

            if (!string.IsNullOrEmpty(outputPath))
            {
                string dlPathWithoutIndex = OutputPathResolver.ResolveOutputFilePath(outputPath, model, mimeType);

                for (int i = 0; i < urls.Count; i++)
                {
                    string dlPath = OutputPathResolver.AddIndexToPath(dlPathWithoutIndex, i);
                    resolvedDlPaths.Add(dlPath);
                }

                for (int i = 0; i < urls.Count; i++)
                {
                    string dlPath = resolvedDlPaths[i];
                    string url = urls[i];

                    if (string.IsNullOrEmpty(url))
                    {
                        Debug.LogWarning($"URL at index {i} is null or empty. Skipping download.");
                        continue;
                    }

                    if (!await UnityDownloader.DownloadFileAsync(url, dlPath))
                    {
                        Debug.LogError($"Failed to download video from {url} to {dlPath}. Please check the URL or your internet connection.");
                    }
                }
            }

            return new GeneratedVideo(urls.ToArray(), resolvedDlPaths.ToArray(), usage);
        }
    }
}
