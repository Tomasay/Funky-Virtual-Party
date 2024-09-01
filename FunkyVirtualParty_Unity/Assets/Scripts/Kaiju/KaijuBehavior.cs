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
    List<Transform> Targets;

    public int currentTarget = 0;

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
            Vector3 diff = Targets[currentTarget].position - gameObject.transform.position;
            diff.y = 0;
            if (diff.x > -0.01 && diff.x < 0.01 && diff.z > -0.01 && diff.z < 0.01)
            {
                currentTarget++;
                if (currentTarget == Targets.Count)
                    currentTarget = 0;
            }

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
        if(KaijuGameSyncer.instance.State.Equals("kill") )
        {
            Destroy(this);
        }

        // this variable controls whether the kaiju is moving
        Debug.Log("Kaiju TakeDamage called.");
        stature = System.Convert.ToInt32(stature > 0) * (stature - (dmg / 2)); // Why do I have to do this conversion????
        health -= System.Convert.ToInt32(stature < 0) * dmg;
        Debug.Log("Remaining Health: " + health + " Stature: " + stature);


    }

    private void OnCollisionEnter(Collision collision)
    {
        if( collision.gameObject.transform.root.tag.Equals("Player") )
        {
            TakeDamage(10);
        }
    }


}
