using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraKaijuState : MonoBehaviour
{
    CinemachineFreeLook cam;
    [SerializeField] Transform player;
    [SerializeField] Transform XRplayer;
    [SerializeField] Transform kaiju;
    Transform mid;
    KaijuGameClientPlayer clientPlayer;

    float valueToLerpTo = 0;
    float maxRotation = 25;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<CinemachineFreeLook>();
        if (!player)
        {
            player = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        }
        if(!clientPlayer)
        {
            clientPlayer = ClientManagerWeb.instance.LocalPlayer as KaijuGameClientPlayer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!player)
        {
            player = ClientManagerWeb.instance.LocalPlayer.Anim.transform;
        }
        if (!clientPlayer)
        {
            clientPlayer = ClientManagerWeb.instance.LocalPlayer as KaijuGameClientPlayer;
        }

        switch (clientPlayer.state)
        {
            default:
            case KaijuGameClientPlayer.KaijuClientState.OnGround:
            case KaijuGameClientPlayer.KaijuClientState.Jump:
            float rotation = player.rotation.eulerAngles.y;
            if (rotation > 0 && rotation < 90)
            {
                valueToLerpTo = (rotation / 90) * maxRotation;
            }
            else if (rotation > 270 && rotation < 360)
            {
                valueToLerpTo = -((rotation - 270) / 90) * maxRotation;
            }
                cam.m_XAxis.Value = Mathf.Lerp(cam.m_XAxis.Value, valueToLerpTo, Time.deltaTime);
                break;
            case KaijuGameClientPlayer.KaijuClientState.Grabbed:
                // look at vr player while keeping player in frame
                Vector3 m = (XRplayer.position + player.position) / 2f;
                mid.position = m;
                //cam.LookAt = mid;

                Vector3 dist = mid.position - player.position;
                cam.transform.position = player.position - dist;
                Vector3 direction = mid.position - cam.transform.position;
                cam.transform.rotation = Quaternion.LookRotation(direction);
                break;
            case KaijuGameClientPlayer.KaijuClientState.Thrown:
                // look at Kaiju while keeping player in center
                Vector3 ma = (kaiju.position + player.position) / 2f;
                mid.position = ma;
                //cam.LookAt = mid;

                Vector3 dista = mid.position - player.position;
                cam.transform.position = player.position - dista;
                Vector3 directiona = mid.position - cam.transform.position;
                cam.transform.rotation = Quaternion.LookRotation(directiona);
                break;

        }
        
    }
}
