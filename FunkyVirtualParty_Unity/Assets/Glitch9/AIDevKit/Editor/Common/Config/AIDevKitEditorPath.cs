
using Glitch9.Editor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal class AIDevKitEditorPath
    {
        private const string KCataloguePathFormat = "{0}/AIDevKit/Editor/Save/{1}Catalogue.json";
        private const string kVoiceSampleBasePath = "AIDevKit/Editor/Common/Gizmos/Voice Samples";
        private const string kVoiceSamplePathFormat = "{0}/{1}/" + kVoiceSampleBasePath + "/{2}/{3}.mp3";
        private static string _internalResourcesPathCache = null;
        private static string _modelCataloguePathCache = null;
        private static string _voiceCataloguePathCache = null;

        internal static string FindSnippetsDirectory(Api api)
        {
            string providerName = api.GetInspectorName();
            string glitch9Path = EditorPathUtil.FindGlitch9Path();

            return $"{glitch9Path}/AIDevKit/Runtime/{providerName}/Snippets";
        }

        internal static string GetInternalResourcesPath()
        {
            if (_internalResourcesPathCache == null)
            {
                string glitch9Path = EditorPathUtil.FindGlitch9Path();
                _internalResourcesPathCache = $"{glitch9Path}/AIDevKit/Runtime/Resources";
            }

            return _internalResourcesPathCache;
        }

        internal static string GetVoiceSampleFullPath(Api provider, string id)
        {
            string path = string.Format(kVoiceSamplePathFormat, Application.dataPath, EditorPathUtil.FindGlitch9Path(), provider.ToString(), id);
            return path.FixDoubleAssets().FixSlashes();
        }

        internal static string GetVoiceSampleBasePath()
        {
            string path = $"{Application.dataPath}/{EditorPathUtil.FindGlitch9Path()}/{kVoiceSampleBasePath}";
            return path.FixDoubleAssets().FixSlashes();
        }

        internal static string GetModelCataloguePath()
        {
            if (_modelCataloguePathCache == null)
            {
                string glitch9Dir = EditorPathUtil.FindGlitch9Path();
                _modelCataloguePathCache = string.Format(KCataloguePathFormat, glitch9Dir, "Model");
            }
            return _modelCataloguePathCache;
        }

        internal static string GetVoiceCataloguePath()
        {
            if (_voiceCataloguePathCache == null)
            {
                string glitch9Dir = EditorPathUtil.FindGlitch9Path();
                _voiceCataloguePathCache = string.Format(KCataloguePathFormat, glitch9Dir, "Voice");
            }
            return _voiceCataloguePathCache;
        }
    }
}