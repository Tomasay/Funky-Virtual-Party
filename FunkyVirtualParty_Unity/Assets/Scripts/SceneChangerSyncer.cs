using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;

#if UNITY_ANDROID
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#endif

public class SceneChangerSyncer : RealtimeComponent<SceneChangerSyncModel>
{
#if UNITY_ANDROID
        [SerializeField] private VolumeProfile postProcessingProfile;
#elif UNITY_WEBGL
        [SerializeField] RectTransform fadeRect;
        private float fadeIncrementDistance;
#endif

    public string CurrentScene { get => model.currentScene; set => model.currentScene = value; }

    private void Awake()
    {
        SceneManager.sceneLoaded += FadeInScene;

#if UNITY_WEBGL
        //Set fade rect proper size
        float aspect = (Screen.height / fadeRect.rect.height) * 2;
        fadeRect.sizeDelta = new Vector2(fadeRect.rect.width * aspect, fadeRect.rect.height * aspect);
        fadeIncrementDistance = Screen.width / 8;
#endif
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= FadeInScene;
    }

    protected override void OnRealtimeModelReplaced(SceneChangerSyncModel previousModel, SceneChangerSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            currentModel.currentSceneDidChange -= OnSceneChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
            }

            //Update to match new data


            // Register for events
            currentModel.currentSceneDidChange += OnSceneChange;
        }
    }

#region Variable Callbacks
    void OnSceneChange(SceneChangerSyncModel previousModel, string val)
    {

#if UNITY_WEBGL
        val += "Client";
#endif

        SceneManager.LoadScene(val);
    }
#endregion

    private void FadeInScene(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
#if UNITY_ANDROID
        if (postProcessingProfile.TryGet<ColorAdjustments>(out ColorAdjustments ca))
        {
            for (int i = 0; i < 60; i++)
            {
                float t = (float)i / (float)60;
                ca.postExposure.value = Mathf.Lerp(-10, 0, t);
                yield return new WaitForSeconds(1 / 60);
            }
        }
#elif UNITY_WEBGL
        float val = Screen.width + (Screen.width / 2);
        fadeRect.position = new Vector2(val, fadeRect.position.y);


        for (int i = 0; i <= (val * 2) / fadeIncrementDistance; i++)
        {
            fadeRect.position = new Vector2(val - (i * fadeIncrementDistance), fadeRect.position.y);
            yield return new WaitForSeconds(0.05f);
        }
#endif
    }
}
