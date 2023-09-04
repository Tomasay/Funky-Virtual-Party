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
    [SerializeField] public float minSize, maxSize;
    [SerializeField] public Color minColor, maxColor;
    [SerializeField] public Color emberColor, emberColorBoosted;
    [SerializeField] public Color boostedMainColor, boostedSecondaryColor;
    [SerializeField] SpriteRenderer indicator;

    private int maxIndicatorDistance = 4;
    private int fireballExplosionRange = 2;

    public float scaleForBoosted = 0.8f; //What level currentScale has to be for fireball to become boosted

    public float CurrentScale { get => model.currentScale; set => model.currentScale = value; }
    public bool IsBoosted { get => model.boosted; set => model.boosted = value; }
    public bool IsActive { get => model.active; set => model.active = value; }
    public bool ExplosionTrigger { get => model.explosionTrigger; set => model.explosionTrigger = value; }

    private void Update()
    {
#if UNITY_WEBGL
        if(model.active)
        {
            //Check to see if above iceberg
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxIndicatorDistance))
            {
                if (hit.transform.gameObject.name.Contains("Iceberg_Collider"))
                {
                    //Enable indicator
                    indicator.enabled = true;
                    indicator.transform.position = hit.point + new Vector3(0, 0.1f, 0);
                    indicator.transform.rotation = Quaternion.Euler(Quaternion.identity.eulerAngles + new Vector3(-90, 0, 0));

                    //Color and size
                    float t = hit.distance / maxIndicatorDistance;
                    indicator.transform.localScale = Vector3.one * Mathf.Lerp(0.25f, 2.5f, t);
                    indicator.color = Color.Lerp(Color.red, Color.yellow, t);
                }
                else
                {
                    indicator.enabled = false;
                }
            }
            else
            {
                indicator.enabled = false;
            }
        }
        else
        {
            indicator.enabled = false;
        }
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
            mainFireball.StartColor = Color.Lerp(minColor, maxColor, val);
        }

        float s = Mathf.Lerp(minSize, maxSize, val);

        Vector3 scale = new Vector3(s, s, s);
        fireball.transform.localScale = (s == 0) ? new Vector3(minSize, minSize, minSize) : scale;
        explosion.transform.localScale = scale;
        smokePuff.transform.localScale = scale;
    }

    void OnIsBoostedChange(FireballSyncModel previousModel, bool val)
    {
        mainFireball.StartColor =  val ? boostedMainColor : minColor;
        ember.StartColor = val ? emberColorBoosted : emberColor;
        fireTrail.StartColor = val ? boostedSecondaryColor : emberColor;
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
            RealtimeSingletonWeb.instance.LocalPlayer.GetComponent<ShootoutGameClientPlayer>().CheckCollisionWithFireball(transform.position, fireballExplosionRange, model.currentScale);
#endif

            Invoke("SetExplosionTriggerFalse", 0.5f);
        }
    }
    #endregion

    //Method to reset ExplosionTrigger back to false
    //NOTE these are set back to false with a small delay, as setting them back immediately would cause the initial trigger to not invoke
    void SetExplosionTriggerFalse()
    {
        model.explosionTrigger = false;
    }
}