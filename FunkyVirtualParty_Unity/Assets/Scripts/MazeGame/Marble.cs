using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marble : MonoBehaviour
{
    [SerializeField] GameObject handheldMaze;
    [SerializeField] Collider mazeBounds;
    Rigidbody RB, mazeRB;

    private float threshold = 0.005f, startingMazeHeight;
    private Vector3 previousValidPoint;

    void Start()
    {
        RB = GetComponent<Rigidbody>();
        mazeRB = handheldMaze.GetComponent<Rigidbody>();

        previousValidPoint = GetPosRelativeToMaze();
        startingMazeHeight = GetPosRelativeToMaze().y;

        Debug.Log("Starting height: " + startingMazeHeight);
    }

    void Update()
    {
        //If marble is at incorrect height in maze, or out of the bounds
        if(Mathf.Abs(GetPosRelativeToMaze().y - startingMazeHeight) > threshold || !CheckIfInBounds())
        {
            transform.position = handheldMaze.transform.TransformPoint(previousValidPoint);
            //Debug.Log("OUT OF THRESHOLD, setting new pos to: " + handheldMaze.transform.TransformPoint(previousValidPoint));
            //Debug.Log("New height: " + GetPosRelativeToMaze().y);
        }
        else
        {
            previousValidPoint = GetPosRelativeToMaze();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Collision should be dictated by host
#if UNITY_ANDROID
        if (collision.gameObject.TryGetComponent<MazeGameClientPlayer>(out MazeGameClientPlayer player))
        {
            ClientManager.instance.Manager.Socket.Emit("MethodCallToServer", "MarbleCollision", player.PlayerSocketID);

            player.Anim.SetTrigger("Fall");
            player.TriggerBlinkingAnimation(3);
        }
#endif
    }

    private Vector3 GetPosRelativeToMaze()
    {
        Vector3 relativePoint = handheldMaze.transform.InverseTransformPoint(transform.position);
        return relativePoint;
    }

    private bool CheckIfInBounds()
    {
        return (mazeBounds.bounds.Contains(transform.position));
    }
}