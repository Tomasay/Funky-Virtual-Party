using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KaijuPowerup : MonoBehaviour
{
    public static List<KaijuPowerup> pool;

    public enum PowerupType
    {
        Fireball,
        Bomb,
        Speed,
        Weight
    }

    // Fields
    [SerializeField]
    PowerupType type;


    private void Awake()
    {
        if (pool == null)
        {
            pool = new List<KaijuPowerup>();
        }
        pool.Add(this);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // spin :)
        gameObject.transform.Rotate(new Vector3(0, 180 * Time.deltaTime, 0), UnityEngine.Space.Self);

    }

    private void OnTriggerEnter(Collider other)
    {
        if( other.gameObject.name.Contains("ClientPlayer") )
        {
            // apply effect based on type
            switch(type)
            {
                case PowerupType.Fireball:
                case PowerupType.Bomb:
                case PowerupType.Speed:
                case PowerupType.Weight:
                    break;

            }
            gameObject.SetActive(false);
        }
    }
}
