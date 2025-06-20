using System.Collections.Generic;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Files;
using Glitch9.IO.Networking.RESTApi;
using UnityEngine;

namespace Glitch9.AIDevKit.Examples
{
    public class OpenAIAPIExamples
    {
        public async void ChatCompletionExample()
        {
            // simple text generation
            var request = new ChatCompletionRequest.Builder()
                .SetModel(OpenAIModel.GPT4o_Mini)
                .SetPrompt("Your prompt here")
                .Build();

            ChatCompletion completion = await request.ExecuteAsync();

            string text = completion.ToString(); // or completion.ToString();
        }

        // public async void ChatCompletionWithImagesExample()
        // {
        //     string[] imageUrls = new string[]
        //     {
        //         "https://example.com/image1.jpg",
        //         "https://example.com/image2.jpg",
        //         "https://example.com/image3.jpg"
        //     };

        //     ChatCompletionRequest request = new OpenAIChatCompletionRequest.Builder()
        //         .PushMessage(ChatRole.User, "What is this image?", imageUrls: imageUrls)
        //         .Build();

        //     ChatCompletion completion = await request.ExecuteAsync();

        //     string text = completion.ToString(); // or completion.ToString();
        // }

        // public async void ChatCompletionWithImageFilesExamples()
        // {
        //     File<Texture2D>[] imageFiles = new File<Texture2D>[]
        //     {
        //         // An image file inside the project's Assets folder
        //         new("Assets/image1.jpg"),

        //         // An image file inside the project's Resources folder
        //         new("Resources/image1.jpg"),

        //         // An image file from the local disk
        //         new("D:/Pictures/image1.jpg")
        //     };

        //     ChatCompletionRequest request = new OpenAIChatCompletionRequest.Builder()
        //         .PushMessage(ChatRole.User, "What is this image?", imageFiles: imageFiles)
        //         .Build();

        //     ChatCompletion completion = await request.ExecuteAsync();

        //     string text = completion.ToString(); // or completion.ToString();
        // }

        public async void ChatCompletionStreamExample()
        {
            var request = new ChatCompletionRequest.Builder()
                .SetModel(OpenAIModel.GPT4o_Mini)
                .SetPrompt("Your prompt here")
                .SetStream(true) // Enable streaming.
                .Build();

            // Or you can create a stream handler to handle the streamed response.
            var streamListener = new SingleResponseStreamHandler(
                onReceiveText: (message) => Debug.Log($"Streamed chunk: {message}")
            );

            await request.StreamAsync(streamListener);
        }

        public async void ImageGenerationExample()
        {
            var request = new ImageCreationRequest.Builder()
                .SetModel(OpenAIModel.DallE3)
                .SetPrompt("A cute baby sea otter")
                .SetN(1)
                .SetSize(ImageSize._1024x1024)
                .Build();

            Texture2D[] images = await request.ExecuteAsync();
        }

        public async void ImageEditingExample()
        {
            // Using Texture2D
            var otter = Resources.Load<Texture2D>("otter");
            var mask = Resources.Load<Texture2D>("mask");

            var request = new ImageEditRequest.Builder()
                .SetImage(otter)
                .SetMask(mask)
                .SetPrompt("A cute baby sea otter wearing a beret")
                .SetN(2)
                .SetSize(ImageSize._1024x1024)
                .Build();

            Texture2D[] images1 = await request.ExecuteAsync();

            // Using IFile
            var otterFile = new File<Texture2D>("path/to/otter.png");
            var maskFile = new File<Texture2D>("path/to/mask.png");

            var request2 = new ImageEditRequest.Builder()
                .SetImage(otterFile)
                .SetMask(maskFile)
                .SetPrompt("A cute baby sea otter wearing a beret")
                .SetN(2)
                .SetSize(ImageSize._1024x1024)
                .Build();

            Texture2D[] images2 = await request2.ExecuteAsync();
        }

        public async void ImageVariationExample()
        {
            var image = Resources.Load<Texture2D>("image_edit_original");

            var request = new ImageVariationRequest.Builder()
                .SetImage(image)
                .SetN(2)
                .SetSize(ImageSize._1024x1024)
                .Build();

            Texture2D[] images = await request.ExecuteAsync();
        }

        public async void SpeechExample()
        {
            var request = new SpeechRequest.Builder()
                .SetModel(OpenAIModel.TTS1)
                .SetVoice(OpenAIVoice.Alloy)
                .SetPrompt("The quick brown fox jumped over the lazy dog.")
                .Build();

            AudioClip audioClip = await request.ExecuteAsync();
        }

        public async void EmbeddingsExample()
        {
            var request = new EmbeddingRequest.Builder()
                .SetModel(OpenAIModel.Text_Embedding_Ada_002)
                .SetInput("The food was delicious and the waiter...")
                .SetEncodingFormat(EncodingFormat.Float)
                .Build();

            var result = await request.ExecuteAsync();

            float[] embeddings = result.EmbeddingVector;
        }

        // public async void ModerationsExample()
        // {
        //     var request = new ModerationRequest.Builder()
        //         .SetPrompt("I want to kill them.")
        //         .Build();

        //     var result = await request.ExecuteAsync();

        //     if (result.IsFlagged(out List<SafetyRating> moderationData))
        //     {
        //         foreach (var data in moderationData)
        //         {
        //             Debug.Log($"Flagged: {data.IsFlagged}");
        //             Debug.Log($"Category: {data.Category}");
        //             Debug.Log($"Score: {data.Score}");
        //         }
        //     }

        //     // Or you can just
        //     Debug.Log(result);
        // }

        public async void TranscriptionExample()
        {
            var audioFile = new File<AudioClip>("path/to/speech.mp3");
            var request = new TranscriptionRequest.Builder()
                .SetModel(OpenAIModel.Whisper1)
                .SetFile(audioFile)
                .Build();

            var result = await request.ExecuteAsync();

            Debug.Log(result.Text);
            Debug.Log(result.Language);
            Debug.Log(result.Duration);
        }


        public async void TranslationExample()
        {
            var audioFile = new File<AudioClip>("path/to/speech.mp3");

            var request = new TranslationRequest.Builder()
                .SetModel(OpenAIModel.Whisper1)
                .SetFile(audioFile)
                .Build();

            var result = await request.ExecuteAsync();

            Debug.Log(result);
        }

        public async void AudioRecorderExample()
        {
            var recorder = new AudioRecorder();
            recorder.StartRecording(); // Start recording the voice

            // Wait until the recording is done

            var audioClip = recorder.StopRecording(); // Stop recording and get the audio clip

            // Now you can use the audio clip to request transcription or translation
            // For example:

            var request = new TranscriptionRequest.Builder()
                .SetModel(OpenAIModel.Whisper1)
                .SetFile(audioClip)
                .Build();

            var result = await request.ExecuteAsync();

            Debug.Log(result.Text);
            Debug.Log(result.Language);
            Debug.Log(result.Duration);
        }

        public async void FilesExamples()
        {
            // Upload a file
            var file = new RawFile("path/to/mydata.jsonl");

            var request = new FileUploadRequest.Builder()
                .SetFile(file, UploadPurpose.FineTune)
                .Build();

            var result = await OpenAI.OpenAI.DefaultInstance.Files.UploadAsync(request);

            Debug.Log(result.Id);

            // List all files
            var fileList = await OpenAI.OpenAI.DefaultInstance.Files.ListAsync();

            foreach (var fileData in fileList?.Data)
            {
                Debug.Log(fileData.Id);
                Debug.Log(fileData.CreatedAt);
                Debug.Log(fileData.Purpose);
            }

            // Retrieve a specific file
            var fileData2 = await OpenAI.OpenAI.DefaultInstance.Files.RetrieveAsync("file-abc123");

            Debug.Log(fileData2.Id);

            // Delete a file 
            bool deleted = await OpenAI.OpenAI.DefaultInstance.Files.DeleteAsync("file-abc123");

            if (deleted) Debug.Log("File deleted successfully");

            //  Download a file
            var content = await OpenAI.OpenAI.DefaultInstance.Files.RetrieveFileContentAsync("file-abc123");

            Debug.Log(content);
        }
    }
}