using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

public class VRPlayerController : MonoBehaviour
{
    [SerializeField] protected GameObject forwardDirection;
    [SerializeField] protected AutoHandPlayer ahp = null;

    public AutoHandPlayer Ahp { get { return ahp; } }
}