using System;
using Glitch9.IO.Files;
using UnityEngine;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoFilePrefab : MonoBehaviour
    {
        [SerializeField] private Text fileNameTxt;
        [SerializeField] private DemoButton removeButton;
        public RawFile File { get; private set; }
        public Action<RawFile> onRemove { get; set; }

        public void Initialize(RawFile file)
        {
            this.File = file;
            fileNameTxt.text = file.Name;
            removeButton.onClick += () => onRemove?.Invoke(file);
        }
    }
}