using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlameBender : MonoBehaviour
{
    [SerializeField]
    Rigidbody handRB;

    Rigidbody rb;
    Material mat;

    float updateInterval = 0.1f;

    readonly int _FollowThroughID = Shader.PropertyToID("_FollowThrough");

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mat = GetComponent<MeshRenderer>().material;

        InvokeRepeating("UpdateFireBend", 0, updateInterval);
    }

    void Update()
    {
        Debug.Log("Velocity: " + handRB.velocity);

        //mat.SetFloat(_FollowThroughID, handRB.velocity.x * 2);
    }

    void UpdateFireBend()
    {
        float t = Mathf.Clamp(handRB.velocity.x * -1, -0.5f, 0.5f);
        mat.DOFloat(t, _FollowThroughID, updateInterval);
    }
}