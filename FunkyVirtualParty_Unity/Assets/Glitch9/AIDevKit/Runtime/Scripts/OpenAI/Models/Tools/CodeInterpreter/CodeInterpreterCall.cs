using Newtonsoft.Json;

namespace Glitch9.AIDevKit.OpenAI
{
    /* OpenAI code-interpreter supported files
       For text/ MIME types, the encoding must be one of utf-8, utf-16, or ascii.

       FILE FORMAT	MIME TYPE
       .c	text/x-c
       .cs	text/x-csharp
       .cpp	text/x-c++
       .doc	application/msword
       .docx	application/vnd.openxmlformats-officedocument.wordprocessingml.document
       .html	text/html
       .java	text/x-java
       .json	application/json
       .md	text/markdown
       .pdf	application/pdf
       .php	text/x-php
       .pptx	application/vnd.openxmlformats-officedocument.presentationml.presentation
       .py	text/x-python
       .py	text/x-script.python
       .rb	text/x-ruby
       .tex	text/x-tex
       .txt	text/plain
       .css	text/css
       .js	text/javascript
       .sh	application/x-sh
       .ts	application/typescript
       .csv	application/csv
       .jpeg	image/jpeg
       .jpg	image/jpeg
       .gif	image/gif
       .png	image/png
       .tar	application/x-tar
       .xlsx	application/vnd.openxmlformats-officedocument.spreadsheetml.sheet
       .xml	application/xml or "text/xml"
       .zip	application/zip
    */

    public sealed class CodeInterpreterCall : ToolCall
    {
        [JsonProperty("code_interpreter")] public CodeInterpreterTool CodeInterpreter { get; set; }

        [JsonConstructor]
        public CodeInterpreterCall()
        {
            Type = ToolType.CodeInterpreter;
        }
    }
}