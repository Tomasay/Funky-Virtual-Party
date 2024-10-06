using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalftoneEffect : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] float moveSpeed = 15f;
    
    float currentDistance1 = 10f, currentDistance2 = 10f, currentDistance3 = 10f;

    Vector2 nextPos1, nextPos2, nextPos3;

    float nextPosThreshold = 0.1f;

    void Update()
    {
        //Point 1
        mat.SetVector("Point1", Vector2.MoveTowards(mat.GetVector("Point1"), nextPos1, moveSpeed * Time.deltaTime));
        currentDistance1 = (nextPos1 - (Vector2)mat.GetVector("Point1")).magnitude;
        if (currentDistance1 < nextPosThreshold)
        {
            nextPos1 = new Vector2(
                Random.Range(0.0f, 1.0f),
                Random.Range(0.0f, 1.0f)
            );
        }

        //Point2
        mat.SetVector("Point2", Vector2.MoveTowards(mat.GetVector("Point2"), nextPos2, moveSpeed * Time.deltaTime));
        currentDistance2 = (nextPos2 - (Vector2)mat.GetVector("Point2")).magnitude;
        if (currentDistance2 < nextPosThreshold)
        {
            nextPos2 = GetRandomVector2();
        }

        //Point3
        mat.SetVector("Point3", Vector2.MoveTowards(mat.GetVector("Point3"), nextPos3, moveSpeed * Time.deltaTime));
        currentDistance3 = (nextPos3 - (Vector2)mat.GetVector("Point3")).magnitude;
        if (currentDistance3 < nextPosThreshold)
        {
            nextPos3 = GetRandomVector2();
        }
    }

    public Vector2 GetRandomVector2()
    {
        return new Vector2(
            Random.Range(0.0f, 1.0f),
            Random.Range(0.0f, 1.0f)
        );
    }
}