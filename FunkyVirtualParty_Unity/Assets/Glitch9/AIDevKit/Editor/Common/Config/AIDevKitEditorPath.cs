using System.IO;
using Glitch9.Editor;
using UnityEditor;
using UnityEngine;

namespace Glitch9.AIDevKit.Editor
{
    internal class AIDevKitEditorPath
    {
        private const string kConfigFileNameFormat = "{0}Config.cs";
        private const string KCataloguePathFormat = "{0}/AIDevKit/Editor/Save/{1}Catalogue.json";
        private const string kVoiceSampleBasePath = "AIDevKit/Editor/Common/Gizmos/Voice Samples";
        private const string kVoiceSamplePathFormat = "{0}/{1}/" + kVoiceSampleBasePath + "/{2}/{3}.mp3";
        private static string _internalResourcesPathCache = null;
        private static string _modelCataloguePathCache = null;
        private static string _voiceCataloguePathCache = null;

        internal static string FindConfigFilePath(Api api)
        {
            string providerName = api.GetInspectorName();

            if (api == Api.Google) providerName += "AI";

            string fileName = string.Format(kConfigFileNameFormat, providerName);

            // find the path using the existing VoiceData scriptable object
            // this is to ensure that the path is correct
            // note that the path should be the parent path if the found path because the last folder is the provider name
            string[] guids = AssetDatabase.FindAssets("t:TextAsset");
            if (guids.Length == 0) throw new System.Exception("Could not find any text assets. Please check the path and try again.");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith(fileName))
                {
                    return Path.GetDirectoryName(path);
                }
            }

            throw new System.Exception("Could not find the config file. Please check the path and try again.");
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