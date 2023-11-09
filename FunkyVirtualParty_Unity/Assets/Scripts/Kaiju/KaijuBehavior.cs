using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuBehavior : MonoBehaviour
{
    [SerializeField]
    public int stature = 30;
    public int getup = 100;
    public int offense = 100;
    public int health = 100;
    public int speed = 1;

    [SerializeField]
    Normal.Realtime.RealtimeView realtimeView;
    [SerializeField]
    Normal.Realtime.RealtimeTransform realtimeTransform;

    [SerializeField]
    Transform Target;

    //for regenerating stature
    float counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        realtimeTransform.RequestOwnership();
        realtimeView.RequestOwnership();
        stature = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (KaijuGameSyncer.instance.State.Equals("fight") )
        {
            counter += Time.deltaTime;
            Vector3 diff = Target.position - gameObject.transform.position;
            gameObject.transform.position += System.Convert.ToInt32(stature > 0) * (diff.normalized * speed * Time.deltaTime);
            if (counter > 1)
            {
                if (stature < 30)
                    stature++;

                counter = 0;
            }
        }
    }

    void TakeDamage(int dmg) 
    {
        // this variable controls whether the kaiju is moving
        stature = System.Convert.ToInt32(stature > 0) * (stature - (dmg / 2)); // Why do I have to do this conversion????
        health -= System.Convert.ToInt32(stature < 0) * dmg;


    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.transform.root.tag.Equals("Player") )
        {
            TakeDamage(10);
        }
    }


}
