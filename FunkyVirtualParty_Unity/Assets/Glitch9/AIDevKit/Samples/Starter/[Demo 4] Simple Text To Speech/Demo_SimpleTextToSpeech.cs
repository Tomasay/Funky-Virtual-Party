using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using UnityEngine;

public class Demo_SimpleTextToSpeech : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private UnityEngine.UI.Text promptText;

    async void Start()
    {
        if (audioSource == null || promptText == null)
        {
            Debug.LogError("❌ AudioSource or PromptText is not assigned in the Inspector.");
            return;
        }

        try
        {
            //string prompt = "Hello, this is a text-to-speech demo using AI Dev Kit.";
            string prompt = "The journey of a thousand miles begins with a single step. - Lao Tzu";
            promptText.text = prompt; // Display the prompt in the UI

            audioSource.clip = await prompt
                .GENSpeech()
                .SetModel(OpenAIModel.TTS1) // 👈 Explicitly set model here 
                                            // You can remove this line to use the default model (Tools > Preferences > AI Dev Kit)
                .SetVoice(OpenAIVoice.Ash) // 👈 Explicitly set voice here
                                           // You can remove this line to use the default voice (Tools > Preferences > AI Dev Kit) 
                .SetSpeed(1.0f) // 👈 (Optional) Set the desired speech speed
                .DownloadOutput()
                .ExecuteAsync();

            audioSource.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
        }
    }
}
