using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class CustomizationMirror : MonoBehaviour
{
    [SerializeField]
    Color[] playerColors, playerOutlineColors;

    int colorIndex;

    AvatarCustomizationReferences avatarRefs;

    private void Start()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        avatarRefs = avatar.gameObject.GetComponentInChildren<AvatarCustomizationReferences>();
    }

    [Button]
    public void NextColor()
    {
        colorIndex = (colorIndex >= playerColors.Length - 1) ? 0 : colorIndex + 1;
        StartCoroutine("SetColor");
    }

    [Button]
    public void PreviousColor()
    {
        colorIndex = (colorIndex <= 0) ? playerColors.Length - 1 : colorIndex - 1;
        StartCoroutine("SetColor");
    }

    IEnumerator SetColor()
    {
        avatarRefs.headMatSyncer.SetColorWithTween = "_BaseColor" + ",#" + ColorUtility.ToHtmlStringRGBA(playerColors[colorIndex]) + ",0.25";
        avatarRefs.handsMatSyncer.SetColorWithTween = "_BaseColor" + ",#" + ColorUtility.ToHtmlStringRGBA(playerColors[colorIndex]) + ",0.25";

        yield return new WaitForSeconds(0.1f);

        avatarRefs.headMatSyncer.SetColorWithTween = "_OutlineColor" + ",#" + ColorUtility.ToHtmlStringRGBA(playerOutlineColors[colorIndex]) + ",0.25";
        avatarRefs.handsMatSyncer.SetColorWithTween = "_OutlineColor" + ",#" + ColorUtility.ToHtmlStringRGBA(playerOutlineColors[colorIndex]) + ",0.25";
    }
}