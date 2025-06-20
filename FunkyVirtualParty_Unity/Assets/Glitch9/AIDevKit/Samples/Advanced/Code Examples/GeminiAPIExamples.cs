// using Glitch9.AIDevKit.Google;
// using Glitch9.IO.Files;
// using Glitch9.IO.Networking.RESTApi;
// using System.Collections.Generic;
// using System.Linq;
// using Cysharp.Threading.Tasks;
// using Glitch9.IO.Json.Schema;
// using UnityEngine;

// namespace Glitch9.AIDevKit.Examples
// {
//     public class GeminiAPIExamples
//     {
//         public async void SystemInstructionsExample()
//         {
//             var model = new GenerativeModel(
//                 GoogleModel.Gemini2_0_Flash,
//                 systemInstruction: "You are a cat. Your name is Neko.");

//             var response = await model.GenerateContentAsync("Good morning! How are you?");

//             string text = response.ToString(); // or response.ToString() 
//         }

//         public async void GenerateContentExample()
//         {
//             // Choose a model that's appropriate for your use case.
//             var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash);

//             string prompt = "Write a story about a magic backpack.";

//             var response = await model.GenerateContentAsync(prompt);

//             Debug.Log(response.ToString());
//         }


//         public async void GenerateContentWithVisionExample()
//         {

//             // Choose a model that's appropriate for your use case.
//             var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash);

//             var image1 = new File<Texture2D>("Assets/image1.jpg");

//             var image2 = new File<Texture2D>("Assets/image2.jpg");

//             string prompt = "What's different between these pictures?";

//             var response = await model.GenerateContentAsync(prompt, images: new List<File<Texture2D>> { image1, image2 });
//             Debug.Log(response.ToString());
//         }

//         public async void GenerateContentStreamExample()
//         {
//             // Choose a model that's appropriate for your use case.
//             var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash);

//             string prompt = "Write a story about a magic backpack.";

//             var streamHandler = new ChatStreamHandler(onStream: OnStream);

//             var response = await model.GenerateContentAsync(prompt, streamHandler: streamHandler);
//             return;

//             static void OnStream(string chunk)
//             {
//                 Debug.Log(chunk);
//                 Debug.Log("____________________________________________________________________________________");
//             }
//         }

//         public async void SafetyExample()
//         {

//             // using Glitch9.AIDevKit.Google;

//             var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash);

//             var response = await model.GenerateContentAsync(
//                 "Do these look store-bought or homemade?",
//                 safetySettings: new List<SafetySetting>
//             {
//                 { new SafetySetting(Google.HarmCategory.HateSpeech, HarmBlockThreshold.BlockLowAndAbove) },
//                 { new SafetySetting(Google.HarmCategory.Harassment, HarmBlockThreshold.BlockLowAndAbove) }
//             });
//         }

//         public async void CodeExecutionExample()
//         {
//             var model = new GenerativeModel(
//                 GoogleModel.Gemini2_0_Flash,
//                 tools: "code_execution");

//             var response = await model.GenerateContentAsync(
//                 "What is the sum of the first 50 prime numbers? " +
//                 "Generate and run code for the calculation, and make sure you get all 50.");

//             Debug.Log(response.ToString());
//         }

//         public async void CodeExecutionWithChatExample()
//         {
//             var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash);

//             var chat = model.StartChat();

//             var response = await chat.SendMessageAsync(
//                 "What is the sum of the first 50 prime numbers? " +
//                 "Generate and run code for the calculation, and make sure you get all 50.");

//             Debug.Log(response.GetOutputText());
//         }

//         public async void FunctionCallingWithChatExample()
//         {
//             var model = new GenerativeModel(
//                 GoogleModel.Gemini2_0_Flash,
//                 tools: "set_light_values");

//             // Generate a function call 

//             var chat = model.StartChat();
//             var response = await chat.SendMessageAsync("Dim the lights so the room feels cozy and warm.");
//             Debug.Log(response.ToString());

//             /*
//             # Create a chat session that automatically makes suggested function calls
//             chat = model.start_chat(enable_automatic_function_calling=True)
//              */

//             // Create a chat session that automatically makes suggested function calls
//             chat = model.StartChat(enableAutomaticFunctionCalling: true);
//         }

//         // Set the brightness and color temperature of a room light. (mock API).
//         // brightness: Light level from 0 to 100. Zero is off and 100 is full brightness
//         // colorTemp: Color temperature of the light fixture, which can be `daylight`, `cool` or `warm`.
//         public Dictionary<string, object> SetLightValues(int brightness, string colorTemp)
//         {
//             // Implement the real API call here

//             // Return the set brightness and color temperature.
//             return new Dictionary<string, object>
//             {
//                 { "brightness", brightness },
//                 { "colorTemperature", colorTemp }
//             };
//         }


//         // PowerDiscoBallDelegate
//         public class PowerDiscoBallArg
//         {
//             [JsonSchema("power", Description = "Whether to power the disco ball or not.", Required = true)]
//             public bool Power { get; set; }
//         }

//         public class PowerDiscoBallDelegate : FunctionDelegate<PowerDiscoBallArg, Result<bool>>
//         {
//             public override UniTask<Result<bool>> Invoke(PowerDiscoBallArg argument)
//             {
//                 bool result = PowerDiscoBall(argument.Power);
//                 return UniTask.FromResult(Result<bool>.Success(result));
//             }

//             private bool PowerDiscoBall(bool power)
//             {
//                 // Print the status of the disco ball
//                 Debug.Log($"Disco ball is {(power ? "spinning!" : "stopped.")}");

//                 // Return true to indicate success
//                 return true;
//             }
//         }

//         // StartMusicDelegate
//         public class StartMusicArg
//         {
//             [JsonSchema("energetic", Description = "Whether the music is energetic or not.", Required = true)]
//             public bool Energetic { get; set; }

//             [JsonSchema("loud", Description = "Whether the music is loud or not.", Required = true)]
//             public bool Loud { get; set; }

//             [JsonSchema("bpm", Description = "The beats per minute of the music.", Required = true)]
//             public int Bpm { get; set; }
//         }

//         public class StartMusicDelegate : FunctionDelegate<StartMusicArg, Result<string>>
//         {
//             public override UniTask<Result<string>> Invoke(StartMusicArg argument)
//             {
//                 string result = StartMusic(argument.Energetic, argument.Loud, argument.Bpm);
//                 return UniTask.FromResult(Result<string>.Success(result));
//             }

//             private string StartMusic(bool energetic, bool loud, int bpm)
//             {
//                 // Simulate starting music
//                 return $"Music started with energetic={energetic}, loud={loud}, bpm={bpm}";
//             }
//         }

//         // DimLightsDelegate
//         public class DimLightsArg
//         {
//             [JsonSchema("brightness", Description = "The brightness of the lights, 0.0 is off, 1.0 is full.", Required = true)]
//             public float Brightness { get; set; }
//         }

//         public class DimLightsDelegate : FunctionDelegate<DimLightsArg, Result<bool>>
//         {
//             public override UniTask<Result<bool>> Invoke(DimLightsArg argument)
//             {
//                 bool result = DimLights(argument.Brightness);
//                 return UniTask.FromResult(Result<bool>.Success(result));
//             }

//             private bool DimLights(float brightness)
//             {
//                 // Simulate dimming lights
//                 Debug.Log($"Lights dimmed to brightness level: {brightness}");
//                 return true;
//             }
//         }


//         // public async void FunctionCallingWithToolsExample()
//         // {
//         //     // Now call the model with an instruction that could use all of the specified tools. 

//         //     // Set the model up with tools.
//         //     Tool houseFns = new Tool(
//         //         new PowerDiscoBallDelegate(),
//         //         new StartMusicDelegate(),
//         //         new DimLightsDelegate()
//         //     );

//         //     var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash, tools: houseFns);

//         //     // Call the API.
//         //     var chat = model.StartChat();
//         //     var response = await chat.SendMessageAsync("Turn this place into a party!");

//         //     // Print out each of the function calls requested from this single call.
//         //     foreach (var part in response.Parts)
//         //     {
//         //         if (part.FunctionCall != null)
//         //         {
//         //             var fn = part.FunctionCall;
//         //             var args = string.Join(", ", fn.Args.Select(kv => $"{kv.Key}={kv.Value}"));
//         //             Debug.Log($"{fn.Name}({args})");
//         //         }
//         //     }

//         //     // Simulate the responses from the specified tools.
//         //     var responses = new Dictionary<string, object>
//         //     {
//         //         { "power_disco_ball", true },
//         //         { "start_music", "Never gonna give you up." },
//         //         { "dim_lights", true }
//         //     };

//         //     // Build the response parts.
//         //     var responseParts = responses.Select(kv =>
//         //         new Part(new FunctionResponse(kv.Key, new Dictionary<string, object> { { "result", kv.Value } }))
//         //     ).ToList();

//         //     response = await chat.SendMessageAsync(responseParts);
//         //     Debug.Log(response.ToString());
//         // }


//         // Define the schema for the function arguments.
//         public class CalculatorArg
//         {
//             [JsonSchema("a", Description = "The first number.", Required = true)]
//             public float a { get; set; }

//             [JsonSchema("b", Description = "The second number.", Required = true)]
//             public float b { get; set; }
//         }

//         // Define the function delegate.
//         public class MultiplyDelegate : FunctionDelegate<CalculatorArg, Result<float>>
//         {
//             public override UniTask<Result<float>> Invoke(CalculatorArg argument)
//             {
//                 float result = argument.a * argument.b;
//                 return UniTask.FromResult(Result<float>.Success(result));
//             }
//         }

//         // public async void FunctionCallingWithToolsExample2()
//         // {
//         //     var calculator = new Tool(
//         //         FunctionDeclaration.Create<CalculatorArg>(
//         //             "multiply",
//         //             "Returns the product of two numbers.",
//         //             new MultiplyDelegate()
//         //         )
//         //     );

//         //     var model = new GenerativeModel(GoogleModel.Gemini2_0_Flash, tools: calculator);
//         //     var chat = model.StartChat();

//         //     var response = await chat.SendMessageAsync(
//         //         "What's 234551 X 325552 ?"
//         //         );

//         //     // Execute the function yourself:
//         //     var fc = response.Candidates[0].Content.Parts[0].FunctionCall;
//         //     Debug.Assert(fc.Name == "multiply");

//         //     if (!fc.Args.TryGetValue("a", out var aObj) ||
//         //         !fc.Args.TryGetValue("b", out var bObj) ||
//         //         aObj is not float a || bObj is not float b)
//         //     {
//         //         Debug.LogError("Invalid arguments.");
//         //         return;
//         //     }

//         //     float result = a * b;
//         //     Debug.Log(result);

//         //     // Send the result to the model, to continue the conversation:
//         //     var responseParts = new List<Part>
//         //     {
//         //         new Part(
//         //             new FunctionResponse(
//         //                 "multiply",
//         //                 new Dictionary<string, object> { { "result", result } }))
//         //     };

//         //     response = await chat.SendMessageAsync(responseParts);
//         // }

//         public async void CreateFineTuningModelExample()
//         {
//             var baseModel = (await GenerativeAI.DefaultInstance.Models.ListAsync())?.Data
//                 .FirstOrDefault(m => m.SupportedGenerationMethods.Contains("createTunedModel"));

//             if (baseModel == null)
//             {
//                 Debug.LogError("Base model not found.");
//                 return;
//             }

//             var name = $"generate-num-{UnityEngine.Random.Range(0, 10000)}";

//             var tunedModelRequest = new TunedModel.Builder()
//                 .SetSourceModel(baseModel.Id)
//                 .SetTrainingData(
//                     new TuningExample("1", "2"),
//                     new TuningExample("3", "4"),
//                     new TuningExample("-3", "-2"),
//                     new TuningExample("twenty two", "twenty three"),
//                     new TuningExample("two hundred", "two hundred one"),
//                     new TuningExample("ninety nine", "one hundred"),
//                     new TuningExample("8", "9"),
//                     new TuningExample("-98", "-97"),
//                     new TuningExample("1,000", "1,001"),
//                     new TuningExample("10,100,000", "10,100,001"),
//                     new TuningExample("thirteen", "fourteen"),
//                     new TuningExample("eighty", "eighty one"),
//                     new TuningExample("one", "two"),
//                     new TuningExample("three", "four"),
//                     new TuningExample("seven", "eight"))
//                 .SetName(name)
//                 .SetEpochCount(100)
//                 .SetBatchSize(4)
//                 .SetLearningRate(0.001f)
//                 .Build();

//             var operation =
//                 await GenerativeAI.DefaultInstance.TunedModels.CreateAsync(tunedModelRequest);

//             var model =
//                 await GenerativeAI.DefaultInstance.TunedModels.RetrieveAsync(name);

//             Debug.Log(model.ToString());
//         }

//         public async void EvaluateModelExample()
//         {
//             var model = new GenerativeModel("name");

//             var result = await model.GenerateContentAsync("55");
//             Debug.Log(result.ToString());

//             // '56'

//             result = await model.GenerateContentAsync("123455");
//             Debug.Log(result.ToString());

//             // '123456'

//             result = await model.GenerateContentAsync("four");
//             Debug.Log(result.ToString());

//             // 'five'

//             result = await model.GenerateContentAsync("quatre"); // French 4
//             Debug.Log(result.ToString()); // French 5 is "cinq"

//             // 'cinq'

//             result = await model.GenerateContentAsync("III"); // Roman numeral 3
//             Debug.Log(result.ToString()); // Roman numeral 4 is IV

//             // 'IV'

//             result = await model.GenerateContentAsync("七"); // Japanese 7
//             Debug.Log(result.ToString()); // Japanese 8 is 八!

//             // '八'
//         }

//         public async void UpdateTunedModelExample()
//         {
//             await GenerativeAI.DefaultInstance.TunedModels.UpdateAsync(
//                 "name",
//                 new List<UpdateMask>() { new("description", "This is my model.") });

//             var model = await GenerativeAI.DefaultInstance.TunedModels.RetrieveAsync("name");

//             Debug.Log(model.Description);
//         }

//         public async void DeleteTunedModelExample()
//         {
//             // genai.delete_tuned_model(f'tunedModels/{name}') 
//             await GenerativeAI.DefaultInstance.TunedModels.DeleteAsync("name");
//         }
//     }
// }
