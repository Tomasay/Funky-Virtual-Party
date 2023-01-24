using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientPlayerDebug : MonoBehaviour
{
    ClientPlayer cp;

    void Awake()
    {
        cp = GetComponent<ClientPlayer>();

        Customize();

        ClientManager.instance.onClientConnect += NewPlayerJoined;
    }

    void NewPlayerJoined(GameObject newPlayer)
    {
        ClientManager.instance.Manager.Socket.Emit("syncCustomizationsFromClientDebug", cp.PlayerSocketID, "#" + ColorUtility.ToHtmlStringRGB(cp.PlayerColor), cp.PlayerHeadType, cp.PlayerHeight, -1);
    }

    void Customize()
    {
        //Color
        Color newCol = ClientPlayer.availableColors[Random.Range(0, ClientPlayer.availableColors.Count)];
        cp.PlayerColor = newCol;

        //Head shapes
        int headType = Random.Range(-1, cp.Smr.sharedMesh.blendShapeCount);
        if (headType > -1) //if -1, keep base head shape
        {
            cp.Smr.SetBlendShapeWeight(headType, 100);
        }

        //Height
        cp.PlayerHeight = Random.Range(-0.2f, 0.75f);

        ClientManager.instance.Manager.Socket.Emit("syncCustomizationsFromClientDebug", cp.PlayerSocketID, "#" + ColorUtility.ToHtmlStringRGB(cp.PlayerColor), headType, cp.PlayerHeight, -1);
    }
}