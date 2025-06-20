using System;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace Glitch9.Editor
{
    public class EditorXmlUtil
    {
        public static void XmlValueChange(string xmlPath, string index, string value)
        {
            var doc = XDocument.Load(xmlPath);
            try
            {
                var targets = doc.Descendants("string").Where(p => p.Attribute("name")?.Value == index);
                foreach (var element in targets)
                {
                    if (element.Value != value)
                    {
                        element.Value = value;
                        doc.Save(xmlPath);
                    }
                }
            }
            catch (Exception e) { Debug.LogWarning(e); }
        }
    }
}
