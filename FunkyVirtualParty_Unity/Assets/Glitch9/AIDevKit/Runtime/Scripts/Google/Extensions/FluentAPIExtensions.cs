namespace Glitch9.AIDevKit.Google
{
    public static class FluentAPIExtensions
    {
        // // --- Gemini only ---
        // protected AspectRatio? _aspectRatio;
        // protected PersonGeneration? _personGeneration;

        private const string kAspectRatioKey = "google.aspect_ratio";
        private const string kPersonGenerationKey = "google.person_generation";

        /// <summary>
        /// Sets the aspect ratio of the generated image (Gemini only).
        /// </summary>
        public static TTask SetAspectRatio<TTask>(this TTask task, AspectRatio aspectRatio) where TTask : IGENTask
        {
            task.SetOption(kAspectRatioKey, aspectRatio);
            return task;
        }

        internal static AspectRatio? GetAspectRatio<TTask>(this TTask task) where TTask : IGENTask
        {
            if (task.TryGetOption(kAspectRatioKey, out AspectRatio aspectRatio)) return aspectRatio;
            return null;
        }

        /// <summary>
        /// Sets the person generation option (Gemini only).
        /// </summary>
        public static TTask SetPersonGeneration<TTask>(this TTask task, PersonGeneration personGeneration) where TTask : IGENTask
        {
            task.SetOption(kPersonGenerationKey, personGeneration);
            return task;
        }

        internal static PersonGeneration? GetPersonGeneration<TTask>(this TTask task) where TTask : IGENTask
        {
            if (task.TryGetOption(kPersonGenerationKey, out PersonGeneration personGeneration)) return personGeneration;
            return null;
        }
    }
}