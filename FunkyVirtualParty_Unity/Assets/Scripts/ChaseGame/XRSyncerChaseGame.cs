using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRSyncerChaseGame : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer groundReticle;

    void Update()
    {
        /*
        Debug.DrawRay(Head.transform.position, Vector3.down * 10, Color.green);

        //Check to see if above ground
        if (Physics.Raycast(Head.transform.position, Vector3.down, out RaycastHit hit, 10))
        {
            if (hit.transform.gameObject.name.Contains("Ground"))
            {
                //Enable indicator
                groundReticle.enabled = true;
                groundReticle.transform.position = hit.point + new Vector3(0, 0.1f, 0);
                groundReticle.transform.rotation = Quaternion.Euler(Quaternion.identity.eulerAngles + new Vector3(-90, 0, 0));
            }
            else
            {
                groundReticle.enabled = false;
            }
        }
        else
        {
            groundReticle.enabled = false;
        }
        */
    }
}