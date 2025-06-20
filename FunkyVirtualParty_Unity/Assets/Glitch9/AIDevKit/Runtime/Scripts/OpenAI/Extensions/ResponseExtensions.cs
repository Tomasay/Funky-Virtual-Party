// using Cysharp.Threading.Tasks;
// using Glitch9.IO.Files;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;

// namespace Glitch9.AIDevKit.OpenAI
// {
//     public static class ResponseExtensions
//     {
//         /// <summary>
//         /// Extracts all text values from the given array of MessageContent.
//         /// </summary>
//         /// <param name="content">Array of MessageContent</param>
//         /// <returns>List of extracted text values</returns>
//         public static List<string> GetTexts(this List<ContentPartWrapper> content)
//         {
//             if (content == null) return null;

//             List<string> texts = new();
//             foreach (ContentPartWrapper part in content)
//             {
//                 if (part.ToPart() is TextContentPart textPart)
//                 {
//                     if (!string.IsNullOrEmpty(textPart.Text.Value))
//                     {
//                         texts.Add(textPart.Text.Value);
//                     }
//                 }
//             }

//             return texts;
//         }

//         /// <summary>
//         /// Extracts all image files from the given array of MessageContent.
//         /// </summary>
//         /// <param name="content">Array of MessageContent</param>
//         /// <returns>List of extracted UnityImageFile objects</returns>
//         public static List<File<Texture2D>> GetImageFiles(this ContentPartWrapper[] content)
//         {
//             // if (content == null) return null;

//             // List<File<Texture2D>> images = new();

//             // foreach (ChatContentPart part in content)
//             // {
//             //     if (part.Value is ImageContent imageContent)
//             //     {
//             //         images.Add(imageContent.LocalImage);
//             //     }
//             // }

//             // return images;

//             // TODO: FIX
//             return null;
//         }
//     }
// }
