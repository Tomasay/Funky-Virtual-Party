using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.UI;

public class Demo_SimpleResponse : MonoBehaviour
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
            responseText.text = await "Hello, tell me a joke!"
                .GENResponse()
                .SetModel(OpenAIModel.GPT4o) // 👈 Explicitly set model here 
                                             // You can remove this line to use the default model (Tools > Preferences > AI Dev Kit)
                .ExecuteAsync();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
            responseText.text = "Failed to get response.";
        }
    }
}
