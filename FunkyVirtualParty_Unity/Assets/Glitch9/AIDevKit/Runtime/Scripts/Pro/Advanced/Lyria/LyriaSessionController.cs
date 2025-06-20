using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Google;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.IO.Networking.RESTApi;
using Glitch9.IO.Networking.WebSocket;
using Newtonsoft.Json;
using UnityEngine;

namespace Glitch9.AIDevKit.Advanced.Lyria
{
    // internal class LyriaAudioMessageReceiver : IStreamingAudioEventReceiver
    // {
    //     public Action<float[]> onReceiveAudio;
    //     public Action onReceiveAudioDone;

    //     internal LyriaAudioMessageReceiver(RealtimeAssistant assistant)
    //     {
    //         onReceiveAudio = assistant.OnReceiveAudio;
    //     }

    //     public void OnReceiveAudio(float[] audioData) => onReceiveAudio?.Invoke(audioData);
    //     public void OnReceiveAudioDone() => onReceiveAudioDone?.Invoke();
    // }

    public class LyriaSessionController
    {
        /*

        ## Specifications
        - Output format: Raw 16-bit PCM Audio
        - Sample rate: 48kHz
        - Channels: 2 (stereo)

        ## Controls
        Music generation can be influenced in real time by sending messages containing:

        - WeightedPrompt: A text string describing a musical idea, genre, instrument, mood, or characteristic. Multiple prompts can potentially be supplied to blend influences. 
            See above for more details on how to best prompt Lyria RealTime.

        - MusicGenerationConfig: Configuration for the music generation process, influencing the characteristics of the output audio.). 
            Parameters include:
            - guidance: (float) Range: [0.0, 6.0]. Default: 4.0. Controls how strictly the model follows the prompts. Higher guidance improves adherence to the prompt, but makes transitions more abrupt.
            - bpm: (int) Range: [60, 200]. Sets the Beats Per Minute you want for the generated music. You need to stop/play or reset the context for the model it take into account the new bpm.
            - density: (float) Range: [0.0, 1.0]. Controls the density of musical notes/sounds. Lower values produce sparser music; higher values produce "busier" music.
            - brightness: (float) Range: [0.0, 1.0]. Adjusts the tonal quality. Higher values produce "brighter" sounding audio, generally emphasizing higher frequencies.
            - scale: (Enum) Sets the musical scale (Key and Mode) for the generation. Use the Scale enum values provided by the SDK. You need to stop/play or reset the context for the model it take into account the new scale.
            - mute_bass: (bool) Default: False. Controls whether the model reduces the outputs' bass.
            - mute_drums: (bool) Default: False. Controls whether the model outputs reduces the outputs' drums.
            - only_bass_and_drums: (bool) Default: False. Steer the model to try to only output bass and drums.

        - PlaybackControl: Commands to control playback aspects, such as play, pause, stop or reset the context.

        For bpm, density, brightness and scale, if no value is provided, the model will decide what's best according to your initial prompts.

        More classical parameters like temperature (0.0 to 3.0, default 1.1), top_k (1 to 1000, default 40), and seed (0 to 2 147 483 647, 
        randomly selected by default) are also customizable in the MusicGenerationConfig.        
        
        */


        private readonly WebSocketClient<LyriaServerMessage> _client;

        private readonly string _modelId;
        private readonly string _apiKey;

        public bool IsConnected => _client.WebSocket?.State == WebSocketState.Open;
        private Action<float[]> _onReceiveAudio;

        public LyriaSessionController(string modelId = Lyria.DefaultModelId)
        {
            if (!GenerativeAISettings.Instance.HasApiKey()) throw new NoApiKeyException("LyriaSession requires a valid API key. Please set it in the AIDevKit settings.");
            _apiKey = GenerativeAISettings.Instance.GetApiKey();
            if (string.IsNullOrEmpty(_apiKey)) throw new NoApiKeyException("LyriaSession requires a valid API key. Please set it in the AIDevKit settings.");

            _modelId = modelId;
            if (string.IsNullOrEmpty(_modelId)) _modelId = Lyria.DefaultModelId;

            _client = new WebSocketClient<LyriaServerMessage>(
                "LyriaSession",
                OnMessageReceived,
                GenerativeAI.DefaultInstance.JsonSettings
            );
        }

        public UniTask ConnectAsync() => _client.CreateWebSocketConnectionAsync(FormatUrl());
        private string FormatUrl() => Lyria.UrlFormat.Replace("{apiVersion}", Lyria.ApiVersion)
                                            .Replace("{modelId}", _modelId)
                                            .Replace("{apiKey}", _apiKey);

        public async UniTask DisconnectAsync()
        {
            if (IsConnected)
                await _client.CloseWebSocketConnectionAsync("Lyria session closed");
        }

        /// <summary>
        /// This is temporary method to generate music.
        /// It will be replaced with a more robust implementation in the future.
        /// </summary> 
        public async UniTask GenerateMusicAsync(string prompt, Action<float[]> onReceiveAudio, MusicGenerationConfig config = null)
        {
            if (string.IsNullOrEmpty(prompt))
            {
                Debug.LogWarning("Prompt is empty. Cannot generate music.");
                return;
            }

            _onReceiveAudio = onReceiveAudio;

            await SetPromptsAsync(new Prompt(prompt));
            if (config != null) await SetConfigAsync(config);
            await PlayAsync(); // Start playback after setting prompts and config
        }

        public UniTask SetPromptsAsync(params Prompt[] prompts)
        {
            var message = new
            {
                setWeightedPrompts = new
                {
                    prompts
                }
            };

            return SendAsync(message);
        }

        public async UniTask SetConfigAsync(MusicGenerationConfig config)
        {
            var message = new
            {
                setMusicGenerationConfig = new
                {
                    config
                }
            };

            await SendAsync(message);
            await ResetAsync(); // Reset to apply the new config
        }

        public UniTask PlayAsync() => SendAsync(new { play = new { } });
        public UniTask PauseAsync() => SendAsync(new { pause = new { } });
        public UniTask StopAsync() => SendAsync(new { stop = new { } });
        public UniTask ResetAsync() => SendAsync(new { reset = new { } });

        private async UniTask SendAsync(object message)
        {
            if (!IsConnected)
            {
                Debug.LogWarning("LyriaSession is not connected.");
                return;
            }

            string json = JsonConvert.SerializeObject(message);
            await _client.WebSocket.SendAsync(json, default);
        }

        private void OnMessageReceived(LyriaServerMessage message)
        {
            // TODO: Implement audio buffering / playback logic here
            Debug.Log("Lyria audio message received.");

            if (message == null)
            {
                Debug.LogWarning("Received null message from Lyria server.");
                return;
            }

            if (message.HasData && _onReceiveAudio != null)
            {
                List<string> audioChunksBase64 = message.GetAudioChunksBase64();
                if (audioChunksBase64 == null || audioChunksBase64.Count == 0)
                {
                    Debug.LogWarning("No audio chunks received in the message.");
                    return;
                }

                foreach (var base64Chunk in audioChunksBase64)
                {
                    if (string.IsNullOrEmpty(base64Chunk))
                    {
                        Debug.LogWarning("Received empty audio chunk.");
                        continue;
                    }

                    // Decode the Base64 string to a byte array
                    // byte[] audioBytes = Convert.FromBase64String(base64Chunk);

                    // // Convert byte array to float array (assuming 16-bit PCM)
                    // float[] audioData = new float[audioBytes.Length / 2];
                    // for (int i = 0; i < audioData.Length; i++)
                    // {
                    //     // Convert each 16-bit sample to a float in the range [-1.0, 1.0]
                    //     short sample = BitConverter.ToInt16(audioBytes, i * 2);
                    //     audioData[i] = sample / 32768f; // Normalize to [-1.0, 1.0]
                    // }

                    float[] audioData = AudioProcessor.PCM16ToFloatArray(base64Chunk);
                    if (audioData == null || audioData.Length == 0)
                    {
                        Debug.LogWarning("Decoded audio data is empty.");
                        continue;
                    }

                    _onReceiveAudio.Invoke(audioData);
                    Debug.Log($"Received audio chunk with {audioData.Length} samples.");
                }
            }
        }
    }
}