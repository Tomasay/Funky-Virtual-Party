using UnityEngine;

namespace Glitch9.Editor
{
    [System.AttributeUsage(System.AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class HideIfFalseAttribute : PropertyAttribute
    {
        public string ConditionalSourceField = "";

        public HideIfFalseAttribute(string booleanFieldName)
        {
            this.ConditionalSourceField = booleanFieldName;
        }
    }
}