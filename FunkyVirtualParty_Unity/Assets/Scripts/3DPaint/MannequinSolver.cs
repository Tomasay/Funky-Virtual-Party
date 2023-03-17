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
    Transform xrplayer, xrplayerTrackersParent, character;

    [SerializeField]
    Transform leftController, leftHandRef, rightController, rightHandRef;

    [SerializeField]
    Vector3 leftHandRotationOffset, rightHandRotationOffset;

    [SerializeField]
    float posingHeightOffset;

    [SerializeField]
    SkinnedMeshRenderer skinnedMeshRenderer;

    [SerializeField]
    MeshCollider meshCollider;

    Mesh colliderMesh;

    bool updatePose = false;
    bool poseHeightAdjusted = false;

    public byte[] Serialize()
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                foreach (Transform t in mannequinIK.references.GetTransforms())
                {
                    writer.Write(t.position);
                    writer.Write(t.rotation);
                }

            }
            return m.ToArray();
        }
    }

    private void Update()
    {
        if (updatePose)
        {
            followIK.solver.locomotion.offset = xrplayer.position - character.position;

            Transform[] followTransforms = followIK.references.GetTransforms();
            Transform[] mannequinTransforms = mannequinIK.references.GetTransforms();
            for (int i = 1; i < followTransforms.Length; i++)
            {
                mannequinTransforms[i].localPosition = followTransforms[i].localPosition;
                mannequinTransforms[i].localRotation = followTransforms[i].localRotation;
            }

            leftHandRef.position = leftController.position;
            leftHandRef.rotation = leftController.rotation * Quaternion.Euler(leftHandRotationOffset);

            rightHandRef.position = rightController.position;
            rightHandRef.rotation = rightController.rotation * Quaternion.Euler(rightHandRotationOffset);

            character.position = xrplayerTrackersParent.position;
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

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByteArray", "IKData", Serialize());
        }
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