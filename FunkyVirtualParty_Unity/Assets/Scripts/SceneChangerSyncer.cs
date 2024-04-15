using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Normal.Realtime;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SceneChangerSyncer : RealtimeComponent<SceneChangerSyncModel>
{
    public static SceneChangerSyncer instance;

    [Tooltip("Only needed for Android")]
    [SerializeField] private VolumeProfile postProcessingProfile;

    [Tooltip("Only needed for WebGL")]
    [SerializeField] RectTransform fadeRect;

    private float fadeIncrementDistance;

    public string CurrentScene { get => model.currentScene; set => model.currentScene = value; }

    private void Awake()
    {
        //Singleton instantiation
        if (!instance)
        {
            instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

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
            previousModel.currentSceneDidChange -= OnSceneChange;
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

        if (SceneUtility.GetBuildIndexByScenePath(val) != -1)
        {
            DestroyDiscs();
            SceneManager.LoadScene(val);
        }
#elif UNITY_ANDROID || UNITY_STANDALONE_WIN
        //Unregister current avatar as it will be destroyed on scene change
        if (RealtimeSingleton.instance.Realtime.connected)
        {
            RealtimeSingleton.instance.RealtimeAvatarManager._UnregisterAvatar(RealtimeSingleton.instance.VRAvatar);
        }

        if (SceneUtility.GetBuildIndexByScenePath(val) != -1)
        {
            StartCoroutine(LoadSceneDelayed(val));
        }
#endif
    }

    IEnumerator LoadSceneDelayed(string scene)
    {
        yield return new WaitForSeconds(1);

        DestroyDiscs();

        SceneManager.LoadScene(scene);
    }

    void DestroyDiscs()
    {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
        //Destroy discs
        foreach (GameObject d in RealtimeSingleton.instance.discs)
        {
            if(d)
<<<<<<< HEAD
            Realtime.Destroy(d);
=======
                Realtime.Destroy(d);
>>>>>>> 734c6125b8b0fd87d3ac7857975d1a5c31f2ecf8
        }
        RealtimeSingleton.instance.discs.Clear();
#elif UNITY_WEBGL
        foreach (GameObject d in GameObject.FindGameObjectsWithTag("Vinyl"))
        {
            if(d)
                Realtime.Destroy(d);
        }
#endif
    }

    #endregion

    private void FadeInScene(Scene arg0, LoadSceneMode arg1)
    {
        StartCoroutine("FadeIn");
    }

    IEnumerator FadeIn()
    {
#if UNITY_ANDROID || UNITY_STANDALONE_WIN
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
