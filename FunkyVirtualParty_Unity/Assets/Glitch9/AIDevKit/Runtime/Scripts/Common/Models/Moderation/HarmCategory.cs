using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Glitch9.AIDevKit
{
    /// <summary>
    /// Represents the category of harm that a piece of content may fall into.
    /// This is used in moderation tasks to classify content based on its potential harm.
    /// </summary>
    [JsonConverter(typeof(HarmCategoryConverter))]
    public enum HarmCategory
    {
        /// <summary>
        /// Default value (it's null).
        /// </summary>
        None,

        /// <summary>
        /// Content that is not classified into any specific category.
        /// </summary>
        Unspecified,

        /// <summary>
        /// Negative or harmful comments targeting identity and/or protected attribute.
        /// </summary>
        Derogatory,

        /// <summary>
        /// Content that is rude, disrespectful, or profane.
        /// </summary>
        Toxicity,

        /// <summary>
        /// Describes scenarios depicting violence against 
        /// an individual or group, or general descriptions of gore.
        /// </summary>
        Violence,

        /// <summary>
        /// Contains references to sexual acts or other lewd content.
        /// </summary>
        Sexual,

        /// <summary>
        /// Promotes unchecked medical advice.
        /// </summary>
        Medical,

        /// <summary>
        /// Dangerous content that promotes, facilitates, or encourages harmful acts.
        /// </summary>
        Dangerous,

        /// <summary>
        /// Harassment content.
        /// </summary>
        Harassment,

        /// <summary>
        /// Hate speech and content.
        /// Content that expresses, incites, 
        /// or promotes hate based on race, gender, ethnicity, religion, nationality, sexual orientation, disability status, or caste.
        /// </summary>
        Hate,

        /// <summary>
        /// Sexually explicit content.
        /// </summary>
        SexuallyExplicit,

        /// <summary>
        /// Dangerous content.
        /// </summary>
        DangerousContent,

        /// <summary>
        /// Content meant to arouse sexual excitement, 
        /// such as the description of sexual activity, 
        /// or that promotes sexual services (excluding sex education and wellness).
        /// </summary>
        SexualContent,

        /// <summary>
        /// Content that promotes, encourages, 
        /// or depicts acts of self-harm, such as suicide, cutting, and eating disorders.
        /// </summary>
        SelfHarm,

        /// <summary>
        /// Sexual content that includes an individual who is under 18 years old.
        /// </summary>
        SexualMinors,

        /// <summary>
        /// Hateful content that also includes violence 
        /// or serious harm towards the targeted group based on 
        /// race, gender, ethnicity, religion, nationality, sexual orientation, disability status, or caste.
        /// </summary>
        HateThreatening,

        /// <summary>
        /// Content that depicts death, violence, 
        /// or physical injury in graphic detail.
        /// </summary>
        ViolenceGraphic,

        /// <summary>
        /// Content where the speaker expresses that they are engaging 
        /// or intend to engage in acts of self-harm, such as suicide, cutting, and eating disorders.
        /// </summary>
        SelfHarmIntent,

        /// <summary>
        /// Content that encourages performing acts of self-harm, 
        /// such as suicide, cutting, and eating disorders, 
        /// or that gives instructions or advice on how to commit such acts.
        /// </summary>
        SelfHarmInstructions,

        /// <summary>
        /// Harassment content that also includes violence 
        /// or serious harm towards any target.
        /// </summary>
        HarassmentThreatening
    }

    internal static class HarmCategoryUtil
    {
        internal static bool IsOpenAIOnly(this HarmCategory category)
        {
            return category switch
            {
                HarmCategory.SexualMinors or
                HarmCategory.HateThreatening or
                HarmCategory.ViolenceGraphic or
                HarmCategory.SelfHarm or
                HarmCategory.SelfHarmIntent or
                HarmCategory.SelfHarmInstructions or
                HarmCategory.HarassmentThreatening => true,
                _ => false,
            };
        }

        internal static bool IsGoogleOnly(this HarmCategory category)
        {
            return category switch
            {
                HarmCategory.Derogatory or
                HarmCategory.Toxicity or
                HarmCategory.Medical or
                HarmCategory.Dangerous or
                HarmCategory.SexuallyExplicit or
                HarmCategory.DangerousContent => true,
                _ => false,
            };
        }

        internal static HarmCategory[] GetOpenAIOnlyCategories()
        {
            return new[]
            {
                HarmCategory.SexualMinors,
                HarmCategory.HateThreatening,
                HarmCategory.ViolenceGraphic,
                HarmCategory.SelfHarm,
                HarmCategory.SelfHarmIntent,
                HarmCategory.SelfHarmInstructions,
                HarmCategory.HarassmentThreatening
            };
        }

        internal static HarmCategory[] GetGoogleOnlyCategories()
        {
            return new[]
            {
                HarmCategory.Derogatory,
                HarmCategory.Toxicity,
                HarmCategory.Medical,
                HarmCategory.Dangerous,
                HarmCategory.SexuallyExplicit,
                HarmCategory.DangerousContent
            };
        }

        internal static HarmCategory[] GetSharedCategories()
        {
            return new[]
            {
                HarmCategory.Violence,
                HarmCategory.Sexual,
                HarmCategory.Harassment,
                HarmCategory.Hate
            };
        }

    }

    internal class HarmCategoryConverter : JsonConverter<HarmCategory>
    {
        internal static HarmCategory Parse(string value)
        {
            if (string.IsNullOrEmpty(value)) return HarmCategory.None;

            return value switch
            {
                "HARM_CATEGORY_VIOLENCE" or "violence" => HarmCategory.Violence,
                "HARM_CATEGORY_SEXUAL" or "sexual" => HarmCategory.Sexual,
                "HARM_CATEGORY_HARASSMENT" or "harassment" => HarmCategory.Harassment,
                "HARM_CATEGORY_HATE_SPEECH" or "hate" => HarmCategory.Hate,

                // OpenAI-only
                "sexual/minors" => HarmCategory.SexualMinors,
                "hate/threatening" => HarmCategory.HateThreatening,
                "violence/graphic" => HarmCategory.ViolenceGraphic,
                "self-harm" => HarmCategory.SelfHarm,
                "self-harm/intent" => HarmCategory.SelfHarmIntent,
                "self-harm/instructions" => HarmCategory.SelfHarmInstructions,
                "harassment/threatening" => HarmCategory.HarassmentThreatening,

                // Google-only
                "HARM_CATEGORY_DEROGATORY" => HarmCategory.Derogatory,
                "HARM_CATEGORY_TOXICITY" => HarmCategory.Toxicity,
                "HARM_CATEGORY_MEDICAL" => HarmCategory.Medical,
                "HARM_CATEGORY_DANGEROUS" => HarmCategory.Dangerous,
                "HARM_CATEGORY_SEXUALLY_EXPLICIT" => HarmCategory.SexuallyExplicit,
                "HARM_CATEGORY_DANGEROUS_CONTENT" => HarmCategory.DangerousContent,

                _ => HarmCategory.Unspecified,
            };
        }


        public override HarmCategory ReadJson(JsonReader reader, Type objectType, HarmCategory existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return HarmCategory.None;

            string value = reader.Value?.ToString();
            return Parse(value);
        }

        public override void WriteJson(JsonWriter writer, HarmCategory value, JsonSerializer serializer) => throw new NotImplementedException();
    }
}