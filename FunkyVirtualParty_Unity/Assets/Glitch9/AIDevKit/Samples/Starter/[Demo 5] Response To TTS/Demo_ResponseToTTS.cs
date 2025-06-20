using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.UI;

public class Demo_ResponseToTTS : MonoBehaviour
{
    [SerializeField] private Text responseText;
    [SerializeField] private AudioSource audioSource;

    async void Start()
    {
        if (audioSource == null || responseText == null)
        {
            Debug.LogError("❌ AudioSource or ResponseText is not assigned in the Inspector.");
            return;
        }

        try
        {
            // 1. Generate a wise response
            string reply = await "Say something wise."
                .GENResponse()
                .SetModel(OpenAIModel.GPT4o)
                .ExecuteAsync();

            responseText.text = reply;

            // 2. Convert that response to speech
            AudioClip clip = await reply.GENSpeech()
                .SetModel(OpenAIModel.TTS1)
                .SetVoice(OpenAIVoice.Nova)
                .ExecuteAsync();

            // 3. Play the audio
            audioSource.clip = clip;
            audioSource.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
            responseText.text = "Failed to get response.";
        }
    }
}
