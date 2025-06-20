using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.AIDevKit.Google;
using UnityEngine;
using UnityEngine.UI;

public class Demo_ImageRecognition : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image myImage; // Assign this in the Inspector with an image to recognize
    [SerializeField] private Texture2D[] myImages; // Assign this in the Inspector with an image to recognize
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

            string question = "What occupation did you always dream of being when growing up?";
            int numOfAnswersToGenerate = 5;
            string correctAnswer = "basketball player";

            string testPrompt1 = "You are playing a game of VRtistry, a minigame similar to Pictionary. " +
            "In this game, players on mobile phones have been given the question: " + question + " and " +
            "one of their answers was chosen at random and given to a player in VR who attempted to draw " +
            "and paint on a poseable mannequin to create it. The answer that was chosen and given to the VR player is: \"" + correctAnswer + "\". " +
            "You are looking at the front, back, left, and right views of the VR player's creation of that answer in that order. " +
            "In order to make the game more interesting, I need you to come up with " + numOfAnswersToGenerate + " " +
            "guesses of what you think it is based on the images. Your answers should be distinct from one another. " +
            "Your answers should be one or two words max. Make sure your answers are not too similar to the chosen answer. " +
            "Please give me the answers only, each separated by a comma. " +
            "You may include spaces in the answers, but make sure there are no spaces in between different answers, only commas." +
            "Feel free to use a little humor with some of these answers, as this game is usually played with friends and the " +
            "goal is to make people laugh. Just make sure the answers make sense for the original question and what you see in the images. " +
            "Make sure that your answers aren't too ridiculous, keep them relatively simple. " +
            "Also make sure the answers are natural and simple words/phrases that someone would normally think of and type." +
            "Make sure that your answers are people, verbs, or costumes. " +
            "I'll give you an example scenario. Let's say the question was \"What did you always want to be when growing up?\" " +
            "and the selected mobile answer is 'Basketball Player'. Some good answers would be: " +
            "Athlete, Michael Jordan, Baller, Dunk King, Allstar"; ;

            string testPrompt2 = "You are looking at a virtual mannequin that was posed and drawn on by a player to create " +
            "a prompt given to them, similar to pictionary. You are looking at the front, back, left, and right views " +
            "of the player's creation in that order. Give me " + numOfAnswersToGenerate + " distinct guesses of what you " +
            "think the player tried to create. Your answers should be one or two words max." +
            "Please give me the answers only, each separated by a comma. You may include spaces in the answers, " +
            "but make sure there are no spaces in between different answers, only commas. " +
            "Feel free to use humor with some of these answers, as this game is usually played with friends and the " +
            "goal is to have answers that make people laugh. " +
            "Also make sure the answers are natural and simple words/phrases that someone would normally type.";

            string testPrompt3 = "You are looking at the front, back, left, and right views in that order of a mannequin that has been " +
            "posed and drawn on by a player in a video game. Based on these images, can you give me " + numOfAnswersToGenerate + " " +
            "guesses of what you think the player tried to create? Try to use a little humor and give me some funny answers, " +
            "but also keep them realistic and not ridiculous. " + 
            //"The correct answer is " + correctAnswer + " so make sure not give me answers that are identical or too similar to that. " +
            "Your answers should be one or two words max. Please give me the answers only, each separated by a comma. " +
            "You may include spaces in the answers, but make sure there are no spaces in between different answers, only commas.";

            //OpenAI Example
            string reply = await testPrompt1
                .GENResponse()
                .SetModel(OpenAIModel.GPT4o_Mini)
                .Attach(myImages) // Attach the image texture for recognition
                .ExecuteAsync();

            responseText.text = reply;

            /*
            AudioClip clip = await reply.GENSpeech()
                .SetModel(OpenAIModel.TTS1)
                .SetVoice(OpenAIVoice.Nova)
                .ExecuteAsync();

            audioSource.clip = clip;
            audioSource.Play();
            */

            //Gemini Example
            /*
            string reply = await "Tell me about this image."
                .GENResponse()
                .SetModel(GoogleModel.Gemini2_0_Flash)
                .Attach(myImage.sprite.texture) // Attach the image texture for recognition
                .ExecuteAsync();

            responseText.text = reply;

            Debug.Log("Reply: " + reply);
            */
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
            responseText.text = "Failed to get response.";
        }
    }
}
