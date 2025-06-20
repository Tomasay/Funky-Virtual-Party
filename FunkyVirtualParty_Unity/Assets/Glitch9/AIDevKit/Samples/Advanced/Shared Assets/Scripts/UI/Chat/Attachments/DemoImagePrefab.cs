using Cysharp.Threading.Tasks;
using Glitch9.IO.Files;
using System;
using UnityEngine;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoImagePrefab : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Image image;
        [SerializeField] private DemoButton removeButton;
        public File<Texture2D> Image { get; private set; }

        public async UniTask Initialize(File<Texture2D> image, Action<File<Texture2D>> onRemove)
        {
            Image = image;
            Texture2D tex = await image.LoadAssetAsync();
            this.image.sprite = tex.ToSprite();
            removeButton.onClick += () => onRemove?.Invoke(image);
        }
    }
}