using UnityEngine;

public class BackgroundDotsNoise : MonoBehaviour
{
    [SerializeField] BoxCollider volume;
    [SerializeField] float moveSpeed = 15f;
    [SerializeField] float currentDSistance = 10f;

    Vector3 nextPos;

    float nextPosThreshold = 0.1f;

    void Start()
    {
        GetRandomPointInsideCollider();
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPos, moveSpeed * Time.deltaTime);

        currentDSistance = (nextPos - transform.position).magnitude;
        if ((nextPos - transform.position).magnitude < nextPosThreshold)
        {
            GetRandomPointInsideCollider();
        }
    }

    public void GetRandomPointInsideCollider()
    {
        Vector3 extents = volume.size / 2f;
        Vector3 point = new Vector3(
            Random.Range(-extents.x, extents.x),
            Random.Range(-extents.y, extents.y),
            Random.Range(-extents.z, extents.z)
        ) + volume.center;

        nextPos = volume.transform.TransformPoint(point);
    }
}