using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.CoreLib.IO.Audio;
using UnityEngine;

public class Demo_TextToSpeechStreaming : MonoBehaviour
{
    [SerializeField] private StreamingAudioPlayer streamingAudioPlayer;
    [SerializeField] private UnityEngine.UI.Text promptText;

    async void Start()
    {
        if (streamingAudioPlayer == null || promptText == null)
        {
            Debug.LogError("❌ AudioSource or PromptText is not assigned in the Inspector.");
            return;
        }

        try
        {
            //string prompt = "Hello, this is a text-to-speech demo using AI Dev Kit.";
            string prompt = "The journey of a thousand miles begins with a single step. - Lao Tzu";
            promptText.text = prompt; // Display the prompt in the UI

            await prompt
                .GENSpeech()
                .SetModel(OpenAIModel.TTS1) // 👈 Explicitly set model here 
                                            // You can remove this line to use the default model (Tools > Preferences > AI Dev Kit)
                .SetVoice(OpenAIVoice.Ash) // 👈 Explicitly set voice here
                                           // You can remove this line to use the default voice (Tools > Preferences > AI Dev Kit) 
                .SetSpeed(1.0f) // 👈 (Optional) Set the desired speech speed  
                .SetOutputMimeType(Glitch9.IO.Files.MIMEType.PCM) // 👈 Set the output format to PCM. 
                                                                  // Unity does not support other audio streaming formats
                .StreamAsync(streamingAudioPlayer);

            // await prompt
            //     .GENSpeech()
            //     .SetModel(Glitch9.AIDevKit.ElevenLabs.ElevenLabsModel.Eleven_Flash_V2)
            //     .SetVoice(Glitch9.AIDevKit.ElevenLabs.ElevenLabsVoice.Rachel)
            //     .SetSpeed(1.0f)
            //     .SetOutputMimeType(Glitch9.IO.Files.MIMEType.PCM)
            //     .StreamAsync(streamingAudioPlayer);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
        }
    }

    public void OnStreamingStarted()
    {
        Debug.Log("Streaming started.");
    }

    public void OnStreamingStopped()
    {
        Debug.Log("Streaming stopped.");
    }
}
