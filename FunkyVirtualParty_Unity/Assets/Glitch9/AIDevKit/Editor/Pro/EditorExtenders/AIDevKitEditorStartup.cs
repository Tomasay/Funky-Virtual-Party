using Cysharp.Threading.Tasks;
using UnityEditor;

namespace Glitch9.AIDevKit.Editor.Pro
{
    [InitializeOnLoad]
    internal static class AIDevKitEditorStartup
    {
        private const string kCatalogueUpdateKey = "CatalogueUpdater_LastUpdatedAt";
        private static bool _hasRun = false;

        static AIDevKitEditorStartup()
        {
            AIDevKitConfig.IsPro = true;

            // if (EditorPrefs.GetInt(kCatalogueUpdateKey) == System.DateTime.Now.Day) return;

            // EditorPrefs.SetInt(kCatalogueUpdateKey, System.DateTime.Now.Day);

            // EditorApplication.delayCall += async () =>
            // {
            //     await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate);

            //     if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            //     if (!AIDevKitSettings.CheckForModelUpdatesOnStartup) return;

            //     await UniTask.WaitUntil(() => !EditorApplication.isCompiling);

            //     // open a confirmation dialog
            //     if (EditorUtility.DisplayDialog("AI Model Updates",
            //         "Do you want to check for AI model updates now? This will ensure you have the latest models available for use.",
            //         "Update Now", "Later"))
            //     {
            //         await ModelCatalogue.Instance.CheckForUpdatesAsync(false, true);
            //     }
            // };

            // 이미 오늘 한 번 실행했는지 확인
            if (EditorPrefs.GetInt(kCatalogueUpdateKey) == System.DateTime.Now.Day)
                return;

            // update 루프에 등록
            EditorApplication.update += WaitUntilEditorReady;
        }

        private static async void WaitUntilEditorReady()
        {
            // Editor가 초기화 중일 경우 대기
            if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
                return;

            if (_hasRun) return;
            _hasRun = true;

            EditorApplication.update -= WaitUntilEditorReady;

            await UniTask.DelayFrame(5); // 약간 여유를 더 줌

            // 다시 한 번 안전 확인
            if (!AIDevKitSettings.CheckForModelUpdatesOnStartup)
                return;

            EditorPrefs.SetInt(kCatalogueUpdateKey, System.DateTime.Now.Day);

            // 다이얼로그 표시
            if (EditorUtility.DisplayDialog(
                    "AI Model Updates",
                    "Do you want to check for AI model updates now?\nThis will ensure you have the latest models available for use.",
                    "Update Now", "Later"))
            {
                await ModelCatalogue.Instance.CheckForUpdatesAsync(false, true);
            }
        }
    }
}