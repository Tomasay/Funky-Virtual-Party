using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Glitch9.Editor.CodeGen
{
    public class CodeGenBuilder
    {
        private readonly List<string> _usings = new();
        private string _namespace;
        private readonly List<CodeGenClass> _classes = new();
        private readonly List<string> _directiveComments = new();

        //private bool _addReSharperDisableAll = false;

        public CodeGenBuilder AddUsing(string usingName)
        {
            _usings.Add(usingName);
            return this;
        }

        public CodeGenBuilder SetNamespace(string namespaceName)
        {
            _namespace = namespaceName;
            return this;
        }

        public CodeGenBuilder AddClass(
            string className,
            ClassType type = ClassType.Class,
            List<ICodeGenCode> codes = null,
            bool isStatic = false,
            bool isPartial = false,
            AccessModifier accessModifier = AccessModifier.Public,
            List<CodeGenComment> comments = null,
            List<string> superClasses = null,
            List<string> interfaces = null)
        {
            _classes.Add(new CodeGenClass(className, type, codes, isStatic, isPartial, accessModifier, comments, superClasses, interfaces));
            return this;
        }

        public CodeGenBuilder AddEnumClass(
            string className,
            AccessModifier accessModifier = AccessModifier.Public,
            List<CodeGenEnumValue> enums = null,
            List<CodeGenComment> comments = null,
            List<string> superClasses = null)
        {
            List<ICodeGenCode> codes = new();
            if (enums != null) codes.AddRange(enums);
            _classes.Add(new CodeGenClass(className, ClassType.Enum, codes, false, false, accessModifier, comments, superClasses));
            return this;
        }

        public CodeGenBuilder AddCode(
            string className,
            ICodeGenCode code)
        {
            var @class = _classes.Find(c => c.Name == className);
            @class?.AddCode(code);
            return this;
        }

        public CodeGenBuilder AddContString(
            string className,
            string propertyName,
            string value,
            AccessModifier accessModifier = AccessModifier.Public,
            List<CodeGenComment> comments = null,
            List<CodeGenAttribute> attributes = null)
        {
            var @class = _classes.Find(c => c.Name == className);
            @class?.AddCode(new CodeGenVariable("string", propertyName, value: $"\"{value}\"", accessModifier, false, false, true, comments, attributes));
            return this;
        }

        public CodeGenBuilder AddEnumValue(
            string className,
            string valueName,
            List<CodeGenComment> comments = null,
            List<CodeGenAttribute> attributes = null)
        {
            var @class = _classes.Find(c => c.Name == className);
            @class?.AddCode(new CodeGenEnumValue(valueName, comments, attributes));
            return this;
        }

        public CodeGenBuilder AddDirectiveComment(string directiveComment)
        {
            _directiveComments.Add(directiveComment);
            return this;
        }

        public string Build()
        {
            using (StringBuilderPool.Get(out StringBuilder sb))
            {
                foreach (var usingName in _usings)
                {
                    sb.AppendLine($"using {usingName};");
                }

                sb.AppendLine();

                if (_directiveComments.Count > 0)
                {
                    foreach (var directiveComment in _directiveComments)
                    {
                        sb.AppendLine($"// {directiveComment}");
                    }
                }

                int classIndentLevel = 0;

                if (_namespace != null)
                {
                    sb.AppendLine($"namespace {_namespace}");
                    sb.AppendLine("{");
                    classIndentLevel++;
                }

                foreach (var @class in _classes)
                {
                    sb.AppendLine(@class.Build(classIndentLevel));
                    sb.AppendLine();
                }

                if (_namespace != null)
                {
                    sb.AppendLine("}");
                }

                return sb.ToString();
            }
        }

        public void Generate(string path, bool overwrite = true, bool createBackup = true)
        {
            if (string.IsNullOrEmpty(System.IO.Path.GetDirectoryName(path)))
                throw new System.Exception($"Invalid path: {path}");

            if (System.IO.File.Exists(path))
            {
                if (overwrite)
                {
                    if (createBackup)
                    {
                        string backupPath = path + ".bak";
                        if (System.IO.File.Exists(backupPath)) System.IO.File.Delete(backupPath);
                        System.IO.File.Move(path, backupPath);
                    }
                    System.IO.File.Delete(path);
                }
                else
                {
                    throw new System.Exception($"File already exists at {path}");
                }
            }

            var content = Build();

            if (string.IsNullOrEmpty(content))
                throw new System.Exception("The generated content is empty");

            if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(path)))
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
            }

            System.IO.File.WriteAllText(path, content);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Script file generated at: {path}");
        }
    }
}
