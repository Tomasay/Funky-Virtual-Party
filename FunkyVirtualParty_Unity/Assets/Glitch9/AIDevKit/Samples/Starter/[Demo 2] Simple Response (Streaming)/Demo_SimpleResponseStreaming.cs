using Glitch9.AIDevKit;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.UI;

public class Demo_SimpleResponseStreaming : MonoBehaviour
{
    [SerializeField] private Text responseText;

    async void Start()
    {
        if (responseText == null)
        {
            Debug.LogError("❌ ResponseText is not assigned in the Inspector.");
            return;
        }

        try
        {
            responseText.text = ""; // 👈 Clear previous response text

            // Start the AI request with streaming response
            await "Explain the concept of quantum computing in simple terms within 100 words."
                .GENResponse()
                .SetModel("models/gemma-3-27b-it") // 👈 Explicitly set model here 
                                                   // You can remove this line to use the default model (Tools > Preferences > AI Dev Kit) 
                .OnStreamText(text => responseText.text += text) // 👈 Append streamed text to responseText
                .StreamAsync(); // IMPORTANT: Use StreamAsync() not ExecuteAsync()
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
            responseText.text = "Failed to get response.";
        }
    }
}
