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
    [SerializeField] public Rigidbody rb;
    //[SerializeField] public ParticleSystem mainFireball, explosion, smokePuff, ember, fireTrail, boostedParticleEffect;
    [SerializeField] public ParticleSystemSyncer mainFireball, explosion, smokePuff, ember, fireTrail, boostedParticleEffect;
    [SerializeField] public Color minColor, maxColor;
    [SerializeField] public Color emberColor, emberColorBoosted;
    [SerializeField] public Color boostedMainColor, boostedSecondaryColor;

    private int fireballExplosionRange = 1;

    public float scaleForBoosted = 0.8f; //What level currentScale has to be for fireball to become boosted

    public float CurrentScale { get => model.currentScale; set => model.currentScale = value; }
    public bool IsBoosted { get => model.boosted; set => model.boosted = value; }
    public bool IsActive { get => model.active; set => model.active = value; }
    public bool ExplosionTrigger { get => model.explosionTrigger; set => model.explosionTrigger = value; }


    protected override void OnRealtimeModelReplaced(FireballSyncModel previousModel, FireballSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.currentScaleDidChange -= OnScaleChange;
            previousModel.boostedDidChange -= OnIsBoostedChange;
            previousModel.activeDidChange -= OnIsActiveChange;
            previousModel.explosionTriggerDidChange -= OnExplosionTriggered;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
            }

            //Update to match new data
            fireball.SetActive(model.active);

            // Register for events
            currentModel.currentScaleDidChange += OnScaleChange;
            currentModel.boostedDidChange += OnIsBoostedChange;
            currentModel.activeDidChange += OnIsActiveChange;
            currentModel.explosionTriggerDidChange += OnExplosionTriggered;
        }
    }

    #region Variable Callbacks
    void OnScaleChange(FireballSyncModel previousModel, float val)
    {
        if (!IsBoosted)
        {
            mainFireball.StartColor = Color.Lerp(minColor, maxColor, CurrentScale);
        }
    }

    void OnIsBoostedChange(FireballSyncModel previousModel, bool val)
    {
        mainFireball.StartColor = boostedMainColor;
        ember.StartColor = emberColorBoosted;
        fireTrail.StartColor = boostedSecondaryColor;
    }

    void OnIsActiveChange(FireballSyncModel previousModel, bool val)
    {
        fireball.SetActive(val);

        rb.isKinematic = !val;
        rb.useGravity = val;
    }

    void OnExplosionTriggered(FireballSyncModel previousModel, bool val)
    {
        if (val)
        {
            rb.isKinematic = true;

#if UNITY_WEBGL
            RealtimeSingletonWeb.instance.LocalPlayer.GetComponent<ShootoutGameClientPlayer>().CheckCollisionWithFireball(transform.position, Mathf.Max(2, model.currentScale * fireballExplosionRange));
#endif

            ExplosionTrigger = false;
        }
    }
#endregion
}