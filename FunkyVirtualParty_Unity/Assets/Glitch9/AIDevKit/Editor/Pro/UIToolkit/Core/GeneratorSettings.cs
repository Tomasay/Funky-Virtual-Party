using Glitch9.AIDevKit.Advanced.Lyria;
using Glitch9.AIDevKit.ElevenLabs;
using Glitch9.CoreLib.IO.Audio;
using Glitch9.Editor;

namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal abstract class GeneratorSettings<TWindow, TSelf>
        where TWindow : class
        where TSelf : GeneratorSettings<TWindow, TSelf>, new()
    {
        internal readonly EPrefs<float> PromptHeight = new($"{typeof(TWindow).Name}.PromptHeight", 60f);
        internal readonly EPrefs<float> PromptHistoryHeight = new($"{typeof(TWindow).Name}.PromptHistoryHeight", 100f);
        internal readonly EPrefs<string> Model = new($"{typeof(TWindow).Name}.Model", string.Empty); // don't use AIDevKitSettings.DefaultIMG. It's scriptable object reference
        internal readonly EPrefs<int> N = new($"{typeof(TWindow).Name}.N", 1);
        internal readonly EPrefs<bool> UseProjectContext = new($"{typeof(TWindow).Name}.UseProjectContext", true);
    }

    internal class CodeGeneratorSettings<TWindow> : GeneratorSettings<TWindow, CodeGeneratorSettings<TWindow>>
        where TWindow : class
    {
        internal readonly EPrefs<string> Namespace = new($"{typeof(TWindow).Name}.Namespace", string.Empty);
        internal readonly EPrefs<GameGenre> GameGenre = new($"{typeof(TWindow).Name}.GameGenre", 0);
        internal readonly EPrefs<bool> Is2D = new($"{typeof(TWindow).Name}.Is2D", false);
        internal readonly EPrefs<bool> IsMultiplayer = new($"{typeof(TWindow).Name}.IsMultiplayer", false);
        internal readonly EPrefs<bool> IsVR = new($"{typeof(TWindow).Name}.IsVR", false);
    }

    internal class ImageGeneratorSettings<TWindow> : GeneratorSettings<TWindow, ImageGeneratorSettings<TWindow>>
        where TWindow : class
    {
        internal readonly EPrefs<ImageQuality> Quality = new($"{typeof(TWindow).Name}.Quality", ImageQuality.Standard);
        internal readonly EPrefs<ImageSize> Size = new($"{typeof(TWindow).Name}.Size", ImageSize._1024x1024);
        internal readonly EPrefs<ImageStyle> Style = new($"{typeof(TWindow).Name}.Style", ImageStyle.Vivid);
        internal readonly EPrefs<Google.AspectRatio> AspectRatio = new($"{typeof(TWindow).Name}.AspectRatio", Google.AspectRatio.Square);
        internal readonly EPrefs<Google.PersonGeneration> PersonGeneration = new($"{typeof(TWindow).Name}.PersonGeneration", Google.PersonGeneration.None);
        internal readonly EPrefs<ArtStyle> ArtStyle = new($"{typeof(TWindow).Name}.ArtStyle", 0);
        internal readonly EPrefs<GameGenre> GameGenre = new($"{typeof(TWindow).Name}.GameGenre", 0);
        internal readonly EPrefs<GameTheme> GameTheme = new($"{typeof(TWindow).Name}.GameTheme", 0);
    }

    internal class AudioGeneratorSettings<TWindow, TSelf> : GeneratorSettings<TWindow, TSelf>
        where TWindow : class
        where TSelf : AudioGeneratorSettings<TWindow, TSelf>, new()
    {
        internal readonly NullableEPrefs<AudioEncoding> Encoding = new($"{typeof(TWindow).Name}.Encoding");
        internal readonly NullableEPrefs<ElevenLabsOutputFormat> ElevenLabsFormat = new($"{typeof(TWindow).Name}.ElevenLabsFormat");
    }

    internal class SoundFXGeneratorSettings<TWindow> : AudioGeneratorSettings<TWindow, SoundFXGeneratorSettings<TWindow>>
        where TWindow : class
    {
        internal readonly NullableEPrefs<double> Duration = new($"{typeof(TWindow).Name}.Duration");
        internal readonly NullableEPrefs<double> PromptInfluence = new($"{typeof(TWindow).Name}.PromptInfluence");
    }

    internal class SpeechGeneratorSettings<TWindow> : AudioGeneratorSettings<TWindow, SpeechGeneratorSettings<TWindow>>
        where TWindow : class
    {
        internal readonly EPrefs<string> Voice = new($"{typeof(TWindow).Name}.Voice", string.Empty);
        internal readonly NullableEPrefs<float> Speed = new($"{typeof(TWindow).Name}.Speed");
        internal readonly NullableEPrefs<uint> Seed = new($"{typeof(TWindow).Name}.Seed");       // 0 default 
    }

    internal class VideoGeneratorSettings<TWindow> : GeneratorSettings<TWindow, VideoGeneratorSettings<TWindow>>
        where TWindow : class
    {
        internal readonly EPrefs<Google.AspectRatio> AspectRatio = new($"{typeof(TWindow).Name}.AspectRatio", Google.AspectRatio.Square);
        internal readonly EPrefs<Google.PersonGeneration> PersonGeneration = new($"{typeof(TWindow).Name}.PersonGeneration", Google.PersonGeneration.None);
    }

    internal class MusicGeneratorSettings<TWindow> : AudioGeneratorSettings<TWindow, MusicGeneratorSettings<TWindow>>
        where TWindow : class
    {
        internal readonly EPrefs<int> Bpm = new($"{typeof(TWindow).Name}.Bpm", 120);
        internal readonly EPrefs<float> Temperature = new($"{typeof(TWindow).Name}.Temperature", 1.0f);
        internal readonly EPrefs<float> Density = new($"{typeof(TWindow).Name}.Density", 0.5f);
        internal readonly EPrefs<float> Brightness = new($"{typeof(TWindow).Name}.Brightness", 0.5f);
        internal readonly EPrefs<MusicScale> Scale = new($"{typeof(TWindow).Name}.Scale", MusicScale.CMajorAMinor);
    }
}

