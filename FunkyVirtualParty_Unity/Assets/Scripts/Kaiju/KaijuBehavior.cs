using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuBehavior : MonoBehaviour
{
    [SerializeField]
    public int stature = 100;
    public int getup = 100;
    public int offense = 100;
    public int health = 100;
    public int speed = 1;

    [SerializeField]
    Transform Target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 diff = Target.position - gameObject.transform.position;
        Debug.Log(diff);
        gameObject.transform.position += (diff.normalized * speed * Time.deltaTime);
    }

    void TakeDamage(int dmg) 
    {
        // this variable controls whether the kaiju is moving
        stature = System.Convert.ToInt32(stature > 0) * (stature - (dmg / 2)); // Why do I have to do this conversion????

    }
}
