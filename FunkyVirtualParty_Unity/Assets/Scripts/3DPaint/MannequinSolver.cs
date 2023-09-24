using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using RootMotion.FinalIK;
using Autohand;

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
    static bool poseHeightAdjusted;

    private void Start()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated += RealtimeAvatarManager_avatarCreated;
    }

    private void OnDestroy()
    {
        RealtimeSingleton.instance.RealtimeAvatarManager.avatarCreated -= RealtimeAvatarManager_avatarCreated;
    }

    private void RealtimeAvatarManager_avatarCreated(Normal.Realtime.RealtimeAvatarManager avatarManager, Normal.Realtime.RealtimeAvatar avatar, bool isLocalAvatar)
    {
        vrPlayer = avatar.GetComponent<VRtistryVRPlayerController>();

        //Set refs for Follow IK
        followIK.solver.spine.headTarget = vrPlayer.cameraHead;
        followIK.solver.leftArm.target = vrPlayer.leftHandRef;
        followIK.solver.rightArm.target = vrPlayer.rightHandRef;
    }

    private void Update()
    {
        if (updatePose && vrPlayer)
        {
            followIK.solver.locomotion.offset = vrPlayer.transform.position - character.position;

            Transform[] followTransforms = followIK.references.GetTransforms();
            Transform[] mannequinTransforms = mannequinIK.references.GetTransforms();
            for (int i = 1; i < followTransforms.Length; i++)
            {
                mannequinTransforms[i].localPosition = followTransforms[i].localPosition;
                mannequinTransforms[i].localRotation = followTransforms[i].localRotation;
            }

            vrPlayer.leftHandRef.position = vrPlayer.leftController.position;
            vrPlayer.leftHandRef.rotation = vrPlayer.leftController.rotation * Quaternion.Euler(leftHandRotationOffset);

            vrPlayer.rightHandRef.position = vrPlayer.rightController.position;
            vrPlayer.rightHandRef.rotation = vrPlayer.rightController.rotation * Quaternion.Euler(rightHandRotationOffset);

            character.position = vrPlayer.trackerOffsetsParent.position;
        }
    }

    public void SetPose()
    {
        mannequinIK.enabled = false;
        updatePose = false;

        colliderMesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(colliderMesh, true); ;
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