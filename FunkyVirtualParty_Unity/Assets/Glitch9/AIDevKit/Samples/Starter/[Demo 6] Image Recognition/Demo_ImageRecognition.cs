using Glitch9.AIDevKit;
using Glitch9.AIDevKit.Google;
using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.UI;

public class Demo_ImageRecognition : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image myImage; // Assign this in the Inspector with an image to recognize
    [SerializeField] private Text responseText; // Assign this in the Inspector to display the response
    [SerializeField] private AudioSource audioSource;

    async void Start()
    {
        if (myImage == null || audioSource == null || responseText == null)
        {
            Debug.LogError("❌ One or more required components are not assigned in the Inspector.");
            return;
        }

        if (myImage.sprite == null)
        {
            Debug.LogError("❌ myImage does not have a sprite assigned.");
            return;
        }

        try
        {
            /* 
            
            Note: To use an image as a Texture2D for Vision(Image Recognition) requests, make sure:

            ✅ Read/Write Enabled is checked in the texture import settings
            ✅ Compression is set to None

            Without these settings, the texture cannot be accessed or encoded properly at runtime.

            */

            string reply = await "Tell me about this image."
                .GENResponse()
                //.SetModel(OpenAIModel.GPT4o)
                .SetModel(GoogleModel.Gemini2_0_Flash)
                .Attach(myImage.sprite.texture) // Attach the image texture for recognition
                .ExecuteAsync();

            responseText.text = reply;

            // AudioClip clip = await reply.GENSpeech()
            //     .SetModel(OpenAIModel.TTS1)
            //     .SetVoice(OpenAIVoice.Nova)
            //     .ExecuteAsync();

            // audioSource.clip = clip;
            // audioSource.Play();
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
            responseText.text = "Failed to get response.";
        }
    }
}
