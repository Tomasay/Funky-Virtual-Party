using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Digger.Modules.Runtime.Sources;
using Digger.Modules.Core.Sources;

public class Shootout_DestructibleTerrian : MonoBehaviour
{
    [SerializeField] int blastRadius = 20, blastDepth = 2;
    [SerializeField] float startingHeight = 30;

    [SerializeField] DiggerMasterRuntime digger;

    Terrain ter = null;

    public bool explodeEvent;
    public Vector3 explodePos;
    public float fireballScale;

    void Start()
    {
        if(TryGetComponent<Terrain>(out Terrain t))
        {
            ter = t;
        }
        else
        {
            Debug.LogWarning("Terrain not found! Destroying script");
            Destroy(this);
        }

        ResetHeight();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<Fireball>(out Fireball fireball))
        {
            

            List<ContactPoint> contacts = new List<ContactPoint>();
            collision.GetContacts(contacts);
            ContactPoint ray = contacts[0];
            float relativeHitTerX = (ray.point.x - transform.position.x) / ter.terrainData.size.x;
            float relativeHitTerZ = (ray.point.z - transform.position.z) / ter.terrainData.size.z;
            // you have a number between 0 and 1 where your terrain has been hit (multiplied by 100 it would be the percentage of width and length)
            float relativeTerCoordX = ter.terrainData.heightmapResolution * relativeHitTerX;
            float relativeTerCoordZ = ter.terrainData.heightmapResolution * relativeHitTerZ;

            // now you have the relative point of your terrain, but you need to floor this because the number of points on terrain are integer
            int hitPointTerX = (int)relativeTerCoordX;
            int hitPointTerZ = (int)relativeTerCoordZ;

            //Now you can use this point to edit it using SetHeights
            float[,] heights = ter.terrainData.GetHeights(0, 0, ter.terrainData.heightmapResolution, ter.terrainData.heightmapResolution);

            explodePos = new Vector3(ray.point.x, ray.point.y - blastDepth, ray.point.z);
            fireballScale = fireball.currentScale;
            Explosion(fireballScale, explodePos);

            explodeEvent = true;
        }
    }

    public void Explosion(float fs, Vector3 ep)
    {
        digger.ModifyAsyncBuffured(ep, BrushType.Sphere, ActionType.Dig, 0, 1, Mathf.Max(2, fs * blastRadius));
    }

    private void ResetHeight()
    {
        float[,] heights = ter.terrainData.GetHeights(0, 0, ter.terrainData.heightmapResolution, ter.terrainData.heightmapResolution);

        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                int rSquare = ((ter.terrainData.alphamapWidth/2) * (ter.terrainData.alphamapWidth/2));
                float sqMag = (new Vector2(ter.terrainData.alphamapWidth / 2, ter.terrainData.alphamapWidth / 2) - new Vector2(x, y)).sqrMagnitude;
                if (sqMag > rSquare)
                {
                    float portion = (sqMag / rSquare);
                    heights[x, y] = 0;
                }
                else
                {
                    heights[x, y] = startingHeight;
                }
            }
        }

        ter.terrainData.SetHeights(0, 0, heights);
    }
}