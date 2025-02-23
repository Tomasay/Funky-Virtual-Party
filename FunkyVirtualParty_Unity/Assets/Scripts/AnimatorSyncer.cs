using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(Animator))]
public class AnimatorSyncer : RealtimeComponent<AnimatorSyncModel>
{
    Animator anim;

    public string Trigger { get => model.trigger; set => model.trigger = value; }
    public string ToggleBool { get => model.toggleBool; set => model.toggleBool = value; }
    public float AnimOffset { get => model.animOffset; set => model.animOffset = value; }

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    protected override void OnRealtimeModelReplaced(AnimatorSyncModel previousModel, AnimatorSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.triggerDidChange -= OnTriggerChange;
            previousModel.toggleBoolDidChange -= OnToggleBool;
            previousModel.animOffsetDidChange -= OnAnimOffsetChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                
            }

            Invoke("CheckDanceAnim", 1);

            // Register for events
            currentModel.triggerDidChange += OnTriggerChange;
            currentModel.toggleBoolDidChange += OnToggleBool;
            currentModel.animOffsetDidChange += OnAnimOffsetChange;
        }
    }

    void CheckDanceAnim()
    {
        //For clients only, make sure dance animation plays if necessary
        if (transform.root.TryGetComponent<ClientSync>(out ClientSync cs))
        {
            if (cs.IsDancing != -1)
            {
                anim.SetTrigger("Dance" + cs.IsDancing);
            }
        }
    }

    #region Variable Callbacks
    void OnTriggerChange(AnimatorSyncModel previousModel, string val)
    {
        if (!val.Equals(""))
        {
            anim.SetTrigger(val);
        }
    }

    void OnAnimOffsetChange(AnimatorSyncModel previousModel, float val)
    {
        anim.SetFloat("AnimOffset", val);
    }

    void OnToggleBool(AnimatorSyncModel previousModel, string val)
    {
        if (!val.Equals(""))
        {
            anim.SetBool(ToggleBool, !anim.GetBool(ToggleBool));
            Invoke("ResetToggleBool", 1);
        }
    }

    void ResetToggleBool()
    {
        ToggleBool = "";
    }
    #endregion
}