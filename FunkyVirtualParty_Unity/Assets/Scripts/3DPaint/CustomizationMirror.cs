using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public class CustomizationMirror : MonoBehaviour
{
    [SerializeField]
    Camera mirrorCam;

    [SerializeField]
    Color[] playerColors, playerOutlineColors;

    int hatIndex, eyesIndex = 1, facialHairIndex = 1, colorIndex;

    AvatarCustomizationReferences avatarRefs;
    CustomAvatars.RealtimeAvatar avatar;

    [SerializeField]
    float cameraLerpSpeed, cameraHeightOffset;

    private void Start()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.realtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void Update()
    {
        if(avatar)
        {
            // Get direction to target but flatten on Y
            Vector3 direction = avatar.head.position - mirrorCam.transform.position;
            direction.y += cameraHeightOffset;

            if (direction.sqrMagnitude > 0.001f) // Prevent zero-length vector
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                mirrorCam.transform.rotation = Quaternion.Slerp(mirrorCam.transform.rotation, targetRotation, Time.deltaTime * cameraLerpSpeed);
            }
        }
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        avatarRefs = avatar.gameObject.GetComponentInChildren<AvatarCustomizationReferences>();
        this.avatar = avatar;

        int layer = LayerMask.NameToLayer("UIPointer");
        avatarRefs.UIPointerLeft.layer = layer;
        avatarRefs.UIPointerRight.layer = layer;
        avatarRefs.UIPointerPreviewRight.layer = layer;
    }

    [Button]
    public void NextHat()
    {
        hatIndex = (hatIndex >= avatarRefs.hats.Length - 1) ? 0 : hatIndex + 1;
        SetCustomization(avatarRefs.hats, hatIndex);
    }

    [Button]
    public void PreviousHat()
    {
        hatIndex = (hatIndex <= 0) ? avatarRefs.hats.Length - 1 : hatIndex - 1;
        SetCustomization(avatarRefs.hats, hatIndex);
    }

    [Button]
    public void NextEyes()
    {
        eyesIndex = (eyesIndex >= avatarRefs.eyes.Length - 1) ? 0 : eyesIndex + 1;
        SetCustomization(avatarRefs.eyes, eyesIndex);
    }

    [Button]
    public void PreviousEyes()
    {
        eyesIndex = (eyesIndex <= 0) ? avatarRefs.eyes.Length - 1 : eyesIndex - 1;
        SetCustomization(avatarRefs.eyes, eyesIndex);
    }

    [Button]
    public void NextFacialHair()
    {
        facialHairIndex = (facialHairIndex >= avatarRefs.facialHair.Length - 1) ? 0 : facialHairIndex + 1;
        SetCustomization(avatarRefs.facialHair, facialHairIndex);
    }

    [Button]
    public void PreviousFacialHair()
    {
        facialHairIndex = (facialHairIndex <= 0) ? avatarRefs.facialHair.Length - 1 : facialHairIndex - 1;
        SetCustomization(avatarRefs.facialHair, facialHairIndex);
    }

    void SetCustomization(GameObject[] customizationList, int newIndex)
    {
        for (int i = 0; i < customizationList.Length; i++)
        {
            if (customizationList[i] && customizationList[i].TryGetComponent<MeshSyncer>(out MeshSyncer ms))
            {
                ms.Enabled = (i == newIndex);
            }
            else if (customizationList[i] && customizationList[i].TryGetComponent<SpriteSyncer>(out SpriteSyncer ss))
            {
                ss.Enabled = (i == newIndex);
            }
        }
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

        avatarRefs.headMatSyncer.SetSecondColorWithTween = "_OutlineColor" + ",#" + ColorUtility.ToHtmlStringRGBA(playerOutlineColors[colorIndex]) + ",0.25";
        avatarRefs.handsMatSyncer.SetSecondColorWithTween = "_OutlineColor" + ",#" + ColorUtility.ToHtmlStringRGBA(playerOutlineColors[colorIndex]) + ",0.25";
    }
}