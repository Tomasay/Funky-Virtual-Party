using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLerpFollow : MonoBehaviour
{
    CinemachineFreeLook cam;
    [SerializeField] Transform player;

    [SerializeField] Transform editorTestPlayer;

    float valueToLerpTo = 0;
    float maxRotation = 25;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineFreeLook>();
        if(!player)
        {
            player = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
        #if UNITY_EDITOR
            player = editorTestPlayer;
        #endif
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = RealtimeSingletonWeb.instance.LocalPlayer.Anim.transform;
        }


        float rotation = player.rotation.eulerAngles.y;
        if (rotation > 0 && rotation < 90)
        {
            valueToLerpTo = (rotation / 90) * maxRotation;
        }
        else if(rotation > 270 && rotation < 360)
        {
            valueToLerpTo = -((rotation-270) / 90) * maxRotation;
        }

        cam.m_XAxis.Value = Mathf.Lerp(cam.m_XAxis.Value, valueToLerpTo, Time.deltaTime);
    }
}
