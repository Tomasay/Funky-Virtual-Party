using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using RootMotion.FinalIK;
using Autohand;
using NaughtyAttributes;

public class MannequinSolver : MonoBehaviour
{
    //From testing, this was the center eye height and rig scale that felt accurate
    //Using this to determing rig scale for people of other heights
    private const float REFERENCE_HEIGHT = 1.7f;
    private const float REFERENCE_SCALE = 0.425f;

    [SerializeField]
    VRIK followIK, mannequinIK;

    [SerializeField]
    Transform character;

    [SerializeField]
    Vector3 leftHandRotationOffset, rightHandRotationOffset;

    [SerializeField]
    float posingHeightOffset;

    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField]
    MeshCollider meshCollider;

    private VRtistryVRPlayerController vrPlayer;

    Mesh colliderMesh;

    bool updatePose = false;
    public bool poseHeightAdjusted;

    private Transform[] cachedFollowTransforms;
    private Transform[] cachedMannequinTransforms;
    private Quaternion leftHandRotQuat;
    private Quaternion rightHandRotQuat;

    private void Start()
    {
        colliderMesh = new Mesh();
        leftHandRotQuat = Quaternion.Euler(leftHandRotationOffset);
        rightHandRotQuat = Quaternion.Euler(rightHandRotationOffset);
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(CustomAvatars.RealtimeAvatarManager avatarManager, CustomAvatars.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrPlayer = avatar.GetComponent<VRtistryVRPlayerController>();

        //Set refs for Follow IK
        followIK.solver.spine.headTarget = vrPlayer.cameraHead;
        followIK.solver.leftArm.target = vrPlayer.leftHandRef;
        followIK.solver.rightArm.target = vrPlayer.rightHandRef;

        cachedFollowTransforms = followIK.references.GetTransforms();
        cachedMannequinTransforms = mannequinIK.references.GetTransforms();
    }

    private void Update()
    {
        if (updatePose && vrPlayer)
        {
            followIK.solver.locomotion.offset = vrPlayer.transform.position - character.position;

            for (int i = 1; i < cachedFollowTransforms.Length; i++)
            {
                cachedMannequinTransforms[i].localPosition = cachedFollowTransforms[i].localPosition;
                cachedMannequinTransforms[i].localRotation = cachedFollowTransforms[i].localRotation;
            }

            vrPlayer.leftHandRef.position = vrPlayer.leftController.position;
            vrPlayer.leftHandRef.rotation = vrPlayer.leftController.rotation * leftHandRotQuat;

            vrPlayer.rightHandRef.position = vrPlayer.rightController.position;
            vrPlayer.rightHandRef.rotation = vrPlayer.rightController.rotation * rightHandRotQuat;

            character.position = vrPlayer.trackerOffsetsParent.position;
        }
    }

    public void SetPose()
    {
        StartCoroutine("TrySettingPose");
    }

    /// <summary>
    /// For some reason, when baking the mesh it will not set properly and the mannequin will get stuck.
    /// The only way I found I was able to relieve this issue is to check if it is stuck,
    /// and continually rebake if so. This issue was introduced when updating to Autohand v4 despite
    /// none of this code being touched in that refactor. No idea why this is necessary but it works :D
    /// </summary>
    /// <returns></returns>
    IEnumerator TrySettingPose()
    {
        int maxAttempts = 10;

        BakeMesh();

        Vector3 compareVector = new Vector3(Mathf.Round(colliderMesh.bounds.center.x * 1000f) / 1000f, Mathf.Round(colliderMesh.bounds.center.y * 100f) / 100f, Mathf.Round(colliderMesh.bounds.center.z * 100f) / 100f);
        //Debug.Log("compareVector: " + compareVector);

        for (int i = 0; i < maxAttempts; i++)
        {
            if(compareVector == Vector3.zero)
            {
                BakeMesh();
                compareVector = new Vector3(Mathf.Round(colliderMesh.bounds.center.x * 1000f) / 1000f, Mathf.Round(colliderMesh.bounds.center.y * 100f) / 100f, Mathf.Round(colliderMesh.bounds.center.z * 100f) / 100f);
                //Debug.Log("compareVector: " + compareVector);
                yield return new WaitForSeconds(0.1f);
            }
        }

        mannequinIK.enabled = false;
        updatePose = false;
    }

    public void BakeMesh()
    {
        skinnedMeshRenderer.BakeMesh(colliderMesh, true);
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = colliderMesh;
    }

    public void EnablePosing()
    {
        mannequinIK.enabled = true;
        updatePose = true;

        if (!poseHeightAdjusted)
        {
            mannequinIK.transform.position += new Vector3(0, posingHeightOffset, 0);
            poseHeightAdjusted = true;
        }

        UpdateHeight();
    }

    void UpdateHeight()
    {
        float newScale = (AutoHandPlayer.Instance.playerHeight * REFERENCE_SCALE) / REFERENCE_HEIGHT;
        character.localScale = new Vector3(newScale, newScale, newScale);
        followIK.solver.scale = newScale;
    }
}