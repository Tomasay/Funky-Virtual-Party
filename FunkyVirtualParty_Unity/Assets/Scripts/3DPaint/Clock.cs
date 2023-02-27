using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    [SerializeField]
    GameObject hourHand, minuteHand;

    float hourAngle, minuteAngle;

    void Start()
    {
        UpdateTime();
        InvokeRepeating("UpdateTime", 1, 1);

        hourHand.transform.localRotation = Quaternion.Euler(0, 0, hourAngle);
        minuteHand.transform.localRotation = Quaternion.Euler(0, 0, minuteAngle);
    }

    private void OnDestroy()
    {
        CancelInvoke("UpdateTime");
    }

    private void Update()
    {
        hourHand.transform.localRotation = Quaternion.Lerp(hourHand.transform.localRotation, Quaternion.Euler(0, 0, hourAngle), Time.deltaTime);
        minuteHand.transform.localRotation = Quaternion.Lerp(minuteHand.transform.localRotation, Quaternion.Euler(0, 0, minuteAngle), Time.deltaTime);
    }

    void UpdateTime()
    {
        hourAngle = ((System.DateTime.Now.Hour % 12) * 30) + ((float)System.DateTime.Now.Minute / 60 * 30);
        minuteAngle = System.DateTime.Now.Minute * 6 + ((float)System.DateTime.Now.Second / 60 * 6);
    }
}