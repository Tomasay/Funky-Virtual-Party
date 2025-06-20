using Glitch9.AIDevKit;
using Glitch9.AIDevKit.OpenAI;
using Glitch9.IO.Json.Schema;
using UnityEngine;
using UnityEngine.UI;

public class Demo_StructuredResponse : MonoBehaviour
{
    [StrictJsonSchema("generated_cat_response", Description = "Schema for structured cat output", Strict = true)]
    public class Cat
    {
        [JsonSchemaProperty("name", Description = "The unique name of the cat", Required = true)]
        public string Name { get; set; }

        [JsonSchemaProperty("breed", Description = "The breed of the cat", Required = true)]
        public string Breed { get; set; }

        [JsonSchemaProperty("color", Description = "The color of the cat", Required = true)]
        public string Color { get; set; }

        [JsonSchemaProperty("age", Description = "The age of the cat in years", Required = true)]
        public int Age { get; set; }

        public override string ToString()
        {
            return $"<color=yellow><b>{Name}</b></color>\n" +
                   $"<color=cyan>Breed:</color> {Breed}\n" +
                   $"<color=green>Color:</color> {Color}\n" +
                   $"<color=orange>Age:</color> {Age} years";
        }
    }

    [SerializeField] private Text cat1;
    [SerializeField] private Text cat2;
    [SerializeField] private Text cat3;

    async void Start()
    {
        if (cat1 == null || cat2 == null || cat3 == null)
        {
            Debug.LogError("❌ One or more required components are not assigned in the Inspector.");
            return;
        }

        try
        {
            Cat[] generatedCats = await "Generate 3 random cats with unique names, breeds, colors, and ages."
                .GENStruct<Cat>()
                .SetCount(3) // Generate 3 cats
                .SetModel(OpenAIModel.GPT4o)
                .ExecuteAsync();

            cat1.text = generatedCats[0].ToString();
            cat2.text = generatedCats[1].ToString();
            cat3.text = generatedCats[2].ToString();

            Debug.Log($"✅ AI request successful: {generatedCats.Length} cats generated.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ AI request failed: {ex.Message}");
        }
    }
}
