using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Shootout_DestructibleTerrian : MonoBehaviour
{
    Terrain ter = null; // assuming this script is attached to the terrain
    // Start is called before the first frame update
    void Start()
    {
       ter = GetComponent<Terrain>(); // assuming this script is attached to the terrain
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        List<ContactPoint> contacts = new List<ContactPoint>(); 
        collision.GetContacts(contacts);
        foreach( var c in contacts )
        {
            //Debug.Log("X: " + c.point.x);
            //Debug.Log("Z: " + c.point.z);
        }
        ContactPoint ray = contacts[0];
        float relativeHitTerX = (ray.point.x - transform.position.x) / ter.terrainData.size.x;
        float relativeHitTerZ = (ray.point.z - transform.position.z) / ter.terrainData.size.z;
        // you have a number between 0 and 1 where your terrain has been hit (multiplied by 100 it would be the percentage of width and length)
        float relativeTerCoordX = ter.terrainData.heightmapResolution * relativeHitTerX;
        float relativeTerCoordZ = ter.terrainData.heightmapResolution * relativeHitTerZ;
        Debug.Log("X: " + relativeHitTerX);
        Debug.Log("Z: " + relativeHitTerZ);
        // now you have the relative point of your terrain, but you need to floor this because the number of points on terrain are integer
        int hitPointTerX = (int)relativeTerCoordX;
        int hitPointTerZ = (int)relativeTerCoordZ;
        Debug.Log("X: " + hitPointTerX);
        Debug.Log("Z: " + hitPointTerZ);
        //Now you can use this point to edit it using SetHeights
        float[,] heights = ter.terrainData.GetHeights(0,0,ter.terrainData.heightmapResolution, ter.terrainData.heightmapResolution);
        heights[hitPointTerX, hitPointTerZ] = 0;

        heights[hitPointTerX, hitPointTerZ - 1] = 0;
        heights[hitPointTerX, hitPointTerZ + 1] = 0;

        heights[hitPointTerX - 1, hitPointTerZ - 1] = 0;
        heights[hitPointTerX + 1, hitPointTerZ - 1] = 0;
        heights[hitPointTerX - 1, hitPointTerZ] = 0;
        heights[hitPointTerX + 1, hitPointTerZ + 1] = 0;
        heights[hitPointTerX - 1, hitPointTerZ + 1] = 0;
        heights[hitPointTerX + 1, hitPointTerZ] = 0;


        ter.terrainData.SetHeights(0, 0, heights);
    }
}
