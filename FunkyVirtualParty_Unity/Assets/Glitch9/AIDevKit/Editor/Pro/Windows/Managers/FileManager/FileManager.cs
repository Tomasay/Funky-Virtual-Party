using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;
using OpenAIClient = Glitch9.AIDevKit.OpenAI.OpenAI;
using GoogleClient = Glitch9.AIDevKit.Google.GenerativeAI;
using UnityEditor;
using Glitch9.Editor;
using Glitch9.IO.Files;

namespace Glitch9.AIDevKit.Editor.Files
{
    public class UploadPurposeSelectDialog : EnumSelectDialog<UploadPurposeSelectDialog, UploadPurpose> { }
    internal static class FileManager
    {
        internal static async UniTask<List<ApiFile>> ReloadFilesAsync()
        {
            FileLibrary.Clear();
            List<ApiFile> retrievedFiles = new();

            if (OpenAISettings.Instance.HasApiKey())
            {
                try
                {
                    QueryResponse<OpenAIFile> queryResponse = await OpenAIClient.DefaultInstance.Files.ListAsync(new(100));
                    OpenAIFile[] files = queryResponse?.Data;
                    if (files.IsNullOrEmpty()) return null;

                    Debug.Log($"Loaded {files.Length} files from OpenAI");

                    foreach (OpenAIFile file in files)
                    {
                        ApiFile fileData = ApiFile.AddToLibrary(file);
                        retrievedFiles.Add(fileData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to retrieve files from OpenAI: {e.Message}");
                }
            }

            if (GenerativeAISettings.Instance.HasApiKey())
            {
                try
                {
                    QueryResponse<GoogleFile> queryResponse = await GoogleClient.DefaultInstance.Files.ListAsync(new(100));
                    GoogleFile[] files = queryResponse?.Data;
                    if (files.IsNullOrEmpty()) return null;

                    Debug.Log($"Loaded {files.Length} files from Google");

                    foreach (GoogleFile file in files)
                    {
                        ApiFile fileData = ApiFile.AddToLibrary(file);
                        retrievedFiles.Add(fileData);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to retrieve files from Google: {e.Message}");
                }
            }

            return retrievedFiles;
        }

        internal static async UniTask<bool> DeleteFileAsync(ApiFile file)
        {
            if (file == null) return false;

            string fileId = file.Id;
            bool success = false;

            if (file.Api == Api.OpenAI)
            {
                try
                {
                    if (!OpenAISettings.Instance.HasApiKey()) throw new Exception("OpenAI API key is not set.");
                    success = await OpenAIClient.DefaultInstance.Files.DeleteAsync(fileId);

                    AIDevKitDebug.Blue($"File {fileId} deleted from OpenAI: {success}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to delete file from OpenAI: {e.Message}");
                }
            }

            if (file.Api == Api.Google)
            {
                try
                {
                    if (!GenerativeAISettings.Instance.HasApiKey()) throw new Exception("Google API key is not set.");
                    string formattedFileId = fileId;
                    if (formattedFileId.StartsWith("files/")) formattedFileId = formattedFileId.Substring(6);
                    success = await GoogleClient.DefaultInstance.Files.DeleteAsync(formattedFileId);

                    AIDevKitDebug.Blue($"File {fileId} deleted from Google: {success}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to delete file from Google: {e.Message}");
                }
            }

            if (success) FileLibrary.Remove(fileId);

            return success;
        }

        internal static async void UploadFile(Api api, Action repaint)
        {
            if (api == Api.OpenAI && !OpenAISettings.Instance.HasApiKey()) throw new Exception("OpenAI API key is not set.");
            if (api == Api.Google && !GenerativeAISettings.Instance.HasApiKey()) throw new Exception("Google API key is not set.");

            string filePath = EditorUtility.OpenFilePanel("Select File", "", "");
            if (string.IsNullOrEmpty(filePath)) return;

            string fileName = System.IO.Path.GetFileName(filePath);
            if (string.IsNullOrEmpty(fileName)) return;

            try
            {
                if (api == Api.OpenAI)
                {
                    UploadPurposeSelectDialog.Show("Select Upload Purpose", "Select the purpose for uploading the file.", async purpose =>
                    {
                        if (purpose == UploadPurpose.Unknown)
                        {
                            Debug.LogWarning("Upload purpose is unknown. File upload cancelled.");
                            return;
                        }

                        RawFile IFile = new(filePath, fileName);

                        FileUploadRequest req = new FileUploadRequest.Builder()
                            .SetFile(IFile, purpose)
                            .Build();

                        OpenAIFile file = await OpenAIClient.DefaultInstance.Files.UploadAsync(req);
                        OnFileUploaded(file, repaint);
                    });
                }
                else if (api == Api.Google)
                {
                    RawFile IFile = new(filePath, fileName, MIMETypeUtil.ParseFromPath(filePath));

                    GoogleFileUploadRequest req = new()
                    {
                        File = IFile,
                        Metadata = new GoogleFileUploadMetadata
                        {
                            Id = fileName,
                            Name = fileName,
                        },
                    };

                    GoogleFile file = await GoogleClient.DefaultInstance.Media.UploadAsync(req);
                    OnFileUploaded(file, repaint);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to upload file: {e.Message}, {e.StackTrace}");
            }
        }

        private static void OnFileUploaded(IApiFile file, Action repaint)
        {
            if (file != null)
            {
                ApiFile fileData = ApiFile.AddToLibrary(file);
                ShowDialog.Info($"File uploaded successfully: {fileData.Id}");
                repaint?.Invoke();
            }
        }

        internal static string ToBytesString(int bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / 1024 / 1024} MB";
            return $"{bytes / 1024 / 1024 / 1024} GB";
        }
    }
}