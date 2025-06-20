using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoUtil
    {
        public static async UniTask CreateImage(int index, File<Texture2D> image, Transform imageContainer)
        {
            const string GAME_OBJECT_NAME = "image_";

            GameObject imageObj = new(GAME_OBJECT_NAME + index);
            Image img = imageObj.AddComponent<Image>();
            Texture2D tex = await image.LoadAssetAsync();

            if (tex != null)
            {
                img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                img.transform.SetParent(imageContainer);
                img.rectTransform.localScale = Vector3.one;
            }
        }

        public static List<Dropdown.OptionData> StringArrayToOptionData(string[] values)
        {
            List<Dropdown.OptionData> optionData = new();
            foreach (string value in values)
            {
                optionData.Add(new Dropdown.OptionData(value));
            }
            return optionData;
        }

        public static List<Dropdown.OptionData> EnumToOptionData<T>() where T : Enum
        {
            List<Dropdown.OptionData> optionData = new();
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                optionData.Add(new Dropdown.OptionData(value.ToString()));
            }
            return optionData;
        }
    }
}