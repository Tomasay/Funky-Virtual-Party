using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class MazeCoin : MonoBehaviour
{
    public static List<MazeCoin> pool;

    ParentConstraint parentConstraint;

    void Awake()
    {
        //Only host needs to worry about pooling
#if UNITY_ANDROID
        if (pool == null)
        {
            pool = new List<MazeCoin>();
        }
        pool.Add(this);
#endif

        //Setup constraint to parent to maze
        parentConstraint = GetComponent<ParentConstraint>();
        ConstraintSource src = new ConstraintSource();
        src.sourceTransform = MazeGameSyncer.instance.maze.transform;
        src.weight = 1;
        parentConstraint.AddSource(src);
        parentConstraint.constraintActive = true;
    }

    private void Update()
    {
        transform.Rotate(0, 10 * Time.deltaTime, 0);
    }
}