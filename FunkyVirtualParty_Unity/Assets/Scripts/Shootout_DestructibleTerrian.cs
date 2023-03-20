using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Digger.Modules.Runtime.Sources;
using Digger.Modules.Core.Sources;
using System.IO;

public class Shootout_DestructibleTerrian : MonoBehaviour
{
    public class ExplosionData
    {
        public Vector3 pos;
        public float size;
    }

    [SerializeField] float blastDepth = 2;
    [SerializeField] float minBlastRadius, maxBlastRadius;
    [SerializeField] float startingHeight = 30;

    [SerializeField] DiggerMasterRuntime digger;

    Terrain ter = null;

    public byte[] Serialize(ExplosionData data)
    {
        using (MemoryStream m = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(m))
            {
                writer.Write(data.pos);
                writer.Write(data.size);

            }
            return m.ToArray();
        }
    }

    public static ExplosionData Deserialize(byte[] data)
    {
        ExplosionData result = new ExplosionData();
        using (MemoryStream m = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(m))
            {
                result.pos = reader.ReadVector3();
                result.size = reader.ReadSingle();
            }
        }
        return result;
    }

    private void Awake()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, byte[]>("MethodCallToClientByteArray", MethodCalledFromServer);
#endif
    }

    private void OnDisable()
    {
#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.Off("MethodCallToClientByteArray");
#endif
    }

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

        //Performing empty dig operation on terrain to reset and fix persisting holes bug
        digger.ModifyAsyncBuffured(new Vector3(-55.5f, 27, 50), BrushType.Sphere, ActionType.Dig, 0, 0, 20);
    }

    void MethodCalledFromServer(string methodName, byte[] data)
    {
        if(methodName.Equals("ExplosionEvent"))
        {
            ExplosionData newData = Deserialize(data);
            digger.ModifyAsyncBuffured(newData.pos, BrushType.Sphere, ActionType.Dig, 0, 1, newData.size);
            CinemachineShake.Instance.ShakeCamera(1, 1);
        }
    }

    public void Explosion(Collision collision, Fireball fireball)
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

        Vector3 explodePos = new Vector3(ray.point.x, ray.point.y - blastDepth, ray.point.z);
        float fireballSize = Mathf.Lerp(minBlastRadius, maxBlastRadius, fireball.currentScale);
        digger.ModifyAsyncBuffured(explodePos, BrushType.Sphere, ActionType.Dig, 0, 1, fireballSize);

        //Send explosion data to clients
        ExplosionData newData = new ExplosionData();
        newData.pos = explodePos;
        newData.size = fireballSize;

        if (ClientManager.instance)
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServerByteArray", "ExplosionEvent", Serialize(newData));
        }
    }

    private void ResetHeight()
    {
        float[,] heights = ter.terrainData.GetHeights(0, 0, ter.terrainData.heightmapResolution, ter.terrainData.heightmapResolution);
        int size = ter.terrainData.heightmapResolution - 1;

        for (int x = 0; x < heights.GetLength(0); x++)
        {
            for (int y = 0; y < heights.GetLength(1); y++)
            {
                int rSquare = ((size / 2) * (size / 2));
                float sqMag = (new Vector2(size / 2, size / 2) - new Vector2(x, y)).sqrMagnitude;
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