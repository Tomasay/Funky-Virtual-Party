using System;
using UnityEngine;

namespace Glitch9
{
    [Serializable]
    public class VariableEntry
    {
        [SerializeField] private string name;
        [SerializeField, SerializeReference] private Variable variable;

        public string Name => name;

        public Variable Variable
        {
            get => variable;
            set => variable = value;
        }

        public VariableEntry(string name, Variable variable)
        {
            this.name = name;
            this.variable = variable;
        }
    }
}