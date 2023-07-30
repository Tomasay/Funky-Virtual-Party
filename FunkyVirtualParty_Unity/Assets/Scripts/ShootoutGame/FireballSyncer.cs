using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;
using UnityEngine.Events;
using TMPro;
using Autohand;

public class FireballSyncer : RealtimeComponent<FireballSyncModel>
{
    [SerializeField] public GameObject fireball;
    [SerializeField] public ParticleSystem mainFireball, explosion, smokePuff, ember, fireTrail, boostedParticleEffect;
    [SerializeField] public Color minColor, maxColor;
    [SerializeField] public Color emberColor, emberColorBoosted;
    [SerializeField] public Color boostedMainColor, boostedSecondaryColor;

    public float scaleForBoosted = 0.8f; //What level currentScale has to be for fireball to become boosted

    private bool isWeb;

    public float CurrentScale { get => model.currentScale; set => model.currentScale = value; }
    public bool IsBoosted { get => model.boosted; set => model.boosted = value; }
    public bool IsActive { get => model.active; set => model.active = value; }
    public bool SmokePuffTrigger { get => model.smokePuffTrigger; set => model.smokePuffTrigger = value; }
    public bool ExplosionTrigger { get => model.explosionTrigger; set => model.explosionTrigger = value; }

    private void Awake()
    {

#if UNITY_WEBGL
        isWeb = true;
#endif
    }


    protected override void OnRealtimeModelReplaced(FireballSyncModel previousModel, FireballSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.currentScaleDidChange -= OnScaleChange;
            previousModel.boostedDidChange -= OnIsBoostedChange;
            previousModel.activeDidChange -= OnIsActiveChange;
            previousModel.smokePuffTriggerDidChange -= OnSmokePuffTriggered;
            previousModel.explosionTriggerDidChange -= OnExplosionTriggered;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it

            //Update to match new data

            // Register for events
            currentModel.currentScaleDidChange += OnScaleChange;
            currentModel.boostedDidChange += OnIsBoostedChange;
            currentModel.activeDidChange += OnIsActiveChange;
            currentModel.smokePuffTriggerDidChange += OnSmokePuffTriggered;
            currentModel.explosionTriggerDidChange += OnExplosionTriggered;
        }
    }

    #region Variable Callbacks
    void OnScaleChange(FireballSyncModel previousModel, float val)
    {
        if (!IsBoosted)
        {
            mainFireball.startColor = Color.Lerp(minColor, maxColor, CurrentScale);
        }
    }

    void OnIsBoostedChange(FireballSyncModel previousModel, bool val)
    {
        boostedParticleEffect.Play();

        mainFireball.startColor = boostedMainColor;
        ember.startColor = emberColorBoosted;
        fireTrail.startColor = boostedSecondaryColor;
    }

    void OnIsActiveChange(FireballSyncModel previousModel, bool val)
    {
        fireball.SetActive(val);
    }

    void OnSmokePuffTriggered(FireballSyncModel previousModel, bool val)
    {
        if (val)
        {
            smokePuff.Play();
            SmokePuffTrigger = false;
        }
    }

    void OnExplosionTriggered(FireballSyncModel previousModel, bool val)
    {
        if (val)
        {
            explosion.Play();
            ExplosionTrigger = false;
        }
    }
    #endregion

}
