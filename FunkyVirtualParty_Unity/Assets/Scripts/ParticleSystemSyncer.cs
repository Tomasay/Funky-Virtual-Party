using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemSyncer : RealtimeComponent<ParticleSystemSyncModel>
{
    ParticleSystem pSystem;
    ParticleSystemRenderer pRenderer;

    public Gradient initialColorOverLifetime;

    public ParticleSystem PSystem { get => pSystem;}
    public Color StartColor { get => model.startColor; set => model.startColor = value; }
    public Color MaterialEmissionColor { get => model.materialEmissionColor; set => model.materialEmissionColor = value; }
    public Color MaterialTintColor { get => model.materialTintColor; set => model.materialTintColor = value; }

    public void Play()
    {
        model.playParticles = true;
    }

    public void Stop()
    {
        model.stopParticles = true;
    }

    private void Awake()
    {
        pSystem = GetComponent<ParticleSystem>();
        pRenderer = GetComponent<ParticleSystemRenderer>();

        initialColorOverLifetime = pSystem.colorOverLifetime.color.gradient;
    }

    protected override void OnRealtimeModelReplaced(ParticleSystemSyncModel previousModel, ParticleSystemSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.playParticlesDidChange -= OnPlayParticlesChange;
            previousModel.stopParticlesDidChange -= OnStopParticlesChange;
            previousModel.startColorDidChange -= OnStartColorChange;
            previousModel.materialEmissionColorDidChange -= OnMaterialEmissionColorChange;
            previousModel.materialTintColorDidChange -= OnMaterialTintColorChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {

            }

            //Update to match new data

            // Register for events
            currentModel.playParticlesDidChange += OnPlayParticlesChange;
            currentModel.stopParticlesDidChange += OnStopParticlesChange;
            currentModel.startColorDidChange += OnStartColorChange;
            currentModel.materialEmissionColorDidChange += OnMaterialEmissionColorChange;
            currentModel.materialTintColorDidChange += OnMaterialTintColorChange;
        }
    }

    #region Variable Callbacks
    void OnPlayParticlesChange(ParticleSystemSyncModel previousModel, bool val)
    {
        if (val && pSystem)
        {
            pSystem.Play();
            Invoke("SetPlayFalse", 0.5f);
        }
    }

    void OnStopParticlesChange(ParticleSystemSyncModel previousModel, bool val)
    {
        if (val && pSystem)
        {
            pSystem.Stop();
            Invoke("SetStopFalse", 0.5f);
        }
    }

    void OnStartColorChange(ParticleSystemSyncModel previousModel, Color val)
    {
        ParticleSystem.MainModule main = pSystem.main;
        main.startColor = val;
    }

    void OnMaterialEmissionColorChange(ParticleSystemSyncModel previousModel, Color val)
    {
        pRenderer.material.SetColor("_EmissionColor", val);
    }

    void OnMaterialTintColorChange(ParticleSystemSyncModel previousModel, Color val)
    {
        pRenderer.material.SetColor("_TintColor", val);
    }
    #endregion


    //Methods to reset play and stop triggers back to false
    //NOTE these are set back to false with a small delay, as setting them back immediately would cause the initial trigger to not invoke
    void SetPlayFalse()
    {
        model.playParticles = false;
    }

    void SetStopFalse()
    {
        model.stopParticles = false;
    }
}