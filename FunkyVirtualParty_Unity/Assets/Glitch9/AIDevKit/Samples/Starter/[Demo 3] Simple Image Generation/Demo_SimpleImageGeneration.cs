using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using UnityEngine;

public class Demo_SimpleImageGeneration : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image generatedImage;
    [SerializeField] private UnityEngine.UI.Text promptText;

    async void Start()
    {
        if (generatedImage == null || promptText == null)
        {
            Debug.LogError("❌ GeneratedImage or PromptText is not assigned in the Inspector.");
            return;
        }

        try
        {
            string prompt = "A cat game character, cute and playful, in a modern setting";
            promptText.text = prompt; // Display the prompt in the UI

            generatedImage.sprite = await prompt
                .GENImage()
                .SetModel(OpenAIModel.DallE3) // 👈 Explicitly set model here
                                              // You can remove this line to use the default model (Tools > Preferences > AI Dev Kit)
                .SetSize(ImageSize._1024x1024) // 👈 (Optional) Set the desired image size
                .ExecuteAsync();

        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
        }
    }
}
