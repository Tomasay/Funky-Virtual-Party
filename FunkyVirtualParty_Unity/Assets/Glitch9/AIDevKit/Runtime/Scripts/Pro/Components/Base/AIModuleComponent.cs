using UnityEngine;

namespace Glitch9.AIDevKit.Components
{
    public abstract class AIModuleComponent : AIDevKitComponent
    {
        [SerializeField] protected string model;
        [SerializeField] protected string outputPath;
        public Model Model { get => model; set => model = value; }
        public string OutputPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(outputPath))
                    outputPath = AIDevKitSettings.OutputPath;
                return outputPath;
            }
            set => outputPath = value;
        }
    }
}