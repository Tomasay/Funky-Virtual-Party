using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.Google;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Image Generator")]
    public class ImageGenerator : AIModuleComponent
    {
        // openai
        [SerializeField] private ImageQuality quality;
        [SerializeField] private ImageSize size;
        [SerializeField] private ImageStyle style;

        // google
        [SerializeField] private AspectRatio aspectRatio;
        [SerializeField] private PersonGeneration personGeneration;

        // common
        [SerializeField] private UnityEvent<Texture2D> onTextureGenerated;

        public async void GenerateImage(string text) => await GenerateImageAsync(text);
        public async UniTask<GeneratedImage> GenerateImageAsync(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                OnError("Prompt text cannot be null or empty.");
                return null;
            }

            if (IsBusy)
            {
                OnError("ImageGenerator is already processing another request.");
                return null;
            }

            Model m = model;
            if (m == null) m = AIDevKitConfig.kDefault_OpenAI_IMG;

            Api api = m.Api;

            GENImageTask task = text.GENImage().SetModel(model);

            if (api == Api.OpenAI)
            {
                task.SetQuality(quality).SetSize(size).SetStyle(style);
            }
            else if (api == Api.Google)
            {
                task.SetAspectRatio(aspectRatio).SetPersonGeneration(personGeneration);
            }

            IsBusy = true;
            try
            {
                GeneratedImage generatedImage = await task.ExecuteAsync()
                    ?? throw new EmptyResponseException("Failed to generate image from text.");

                onTextureGenerated?.Invoke(generatedImage);
                return generatedImage;
            }
            catch (System.Exception ex)
            {
                OnError($"Failed to generate image: {ex.Message}");
                return null;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}