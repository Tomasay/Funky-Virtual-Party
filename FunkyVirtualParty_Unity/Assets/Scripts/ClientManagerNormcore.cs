using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Normal.Realtime;

public class ClientManagerNormcore : MonoBehaviour
{
    [SerializeField]
    Realtime realtime;

    private const int PASSCODE_LENGTH = 4;

    void Start()
    {
        realtime.Connect(GenerateCode());
    }

    private string GenerateCode()
    {
        string newCode = "";
        for (int i = 0; i < PASSCODE_LENGTH; i++)
        {
            newCode += GetRandomLetter();
        }
        return newCode;
    }

    public static char GetRandomLetter()
    {
        return (char)Random.Range(65, 91);
    }
}