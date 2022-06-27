using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundDots : MonoBehaviour
{
    [SerializeField] GameObject[] noisePoints;

    MeshRenderer[] circles;


    // Start is called before the first frame update
    void Start()
    {
        circles = GetComponentsInChildren<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (MeshRenderer c in circles)
        {
            //Get closest noise point
            float closest = 100;
            foreach (GameObject n in noisePoints)
            {
                float dist = Vector3.Distance(c.transform.position, n.transform.position);
                if(dist < closest)
                {
                    closest = dist;
                }
            }

            //Set scale accordingly
            float newScale = Mathf.Lerp(2.0f, 0.5f, closest / 35);
            c.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }
}