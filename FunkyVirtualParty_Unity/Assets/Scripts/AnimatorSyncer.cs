using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

[RequireComponent(typeof(Animator))]
public class AnimatorSyncer : RealtimeComponent<AnimatorSyncModel>
{
    Animator anim;

    public string Trigger { get => model.trigger; set => model.trigger = value; }

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    protected override void OnRealtimeModelReplaced(AnimatorSyncModel previousModel, AnimatorSyncModel currentModel)
    {
        if (previousModel != null)
        {
            // Unregister from events
            previousModel.triggerDidChange -= OnTriggerChange;
        }

        if (currentModel != null)
        {
            // If this is a model that has no data set on it
            if (currentModel.isFreshModel)
            {
                
            }

            //For clients only, make sure dance animation plays if necessary
            if (transform.root.TryGetComponent<ClientSync>(out ClientSync cs))
            {
                Debug.Log("Trigger: " + currentModel.trigger + "  AnimSpeed: " + cs.AnimSpeed);
                if (currentModel.trigger.Contains("Dance") && cs.AnimSpeed == 0)
                {
                    anim.SetTrigger(currentModel.trigger);
                }
            }

            // Register for events
            currentModel.triggerDidChange += OnTriggerChange;
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
    #endregion
}