using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedLogoTrigger : MonoBehaviour
{
    public void Next()
    {
        AnimatedLogoManager.instance.Next();
    }
}