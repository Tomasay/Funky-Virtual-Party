namespace Glitch9.AIDevKit.Editor.UIToolKit
{
    internal class UxmlGeneratorWindow : CodeGeneratorWindowBase<UxmlGeneratorWindow>
    {
        protected override string Language => "uxml";
        protected override string Extension => "uxml";
        protected override string DefaultFileName => "MyComponent";

        protected override string GetInstructionText()
        {
            return @"
Create a Unity UXML file for a UI component.
The UXML file should define the structure of the UI component, including any necessary attributes and elements.
Do not include any C# code or logic in the UXML file.
Only return the UXML content as a string.
                 
Example UXML structure:
<ui:UXML xmlns:ui=""UnityEngine.UIElements"" xmlns:uie=""UnityEditor.UIElements"">
    <ui:VisualElement class=""main-container"">
    <!-- Sidebar -->
        <ui:VisualElement name=""sidebar"" class=""panel left-panel"">
            <ui:Label text=""Options"" class=""section-title"" />
            <ui:Toggle name=""toggle-feature"" label=""Enable Feature"" />
            <ui:Slider name=""volume-slider"" low-value=""0"" high-value=""100"" value=""50"" />
            <ui:EnumField name=""enum-mode"" label=""Mode"" />
            <ui:Foldout text=""Advanced"" value=""false"">
                <ui:IntegerField name=""int-setting"" label=""Max Count"" />
                <ui:FloatField name=""float-setting"" label=""Threshold"" />
            </ui:Foldout>
        </ui:VisualElement>
        <!-- Content Area -->
        <ui:VisualElement name=""content"" class=""panel content-panel"">
            <ui:Toolbar>
                <ui:ToolbarButton name=""refresh-button"" text=""Refresh"" />
                <ui:ToolbarSpacer />
                <ui:ToolbarButton name=""save-button"" text=""Save"" />
            </ui:Toolbar>
            <ui:ScrollView name=""scrollable-content"">
                <ui:VisualElement class=""card"">
                    <ui:Label text=""Entry Title"" class=""card-title"" />
                    <ui:TextField name=""entry-description"" multiline=""true"" />
                </ui:VisualElement>
                <ui:VisualElement class=""card"">
                    <ui:Label text=""Details"" class=""card-title"" />
                    <ui:PropertyField name=""object-reference"" />
                </ui:VisualElement>
            </ui:ScrollView>
        </ui:VisualElement>
        <!-- Footer -->
        <ui:VisualElement name=""footer"" class=""footer"">
            <ui:Button name=""cancel-button"" text=""Cancel"" class=""secondary-button"" />
            <ui:Button name=""apply-button"" text=""Apply"" class=""primary-button"" />
        </ui:VisualElement>
    </ui:VisualElement>
";
        }
    }
}