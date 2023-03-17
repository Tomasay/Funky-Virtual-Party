using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif

public class KaijuGameClientPlayer : ClientPlayer
{
    public bool isThrown = false;
    float collisionTimer = 0.5f;
    const float collisionTimerDefault = 1;
    Vector2 collisionVector;

    const float frictionCoefficient = 0.025f;

    public Camera cam;

    [SerializeField] ParticleSystem waterSplash, iceTrail;

    [SerializeField] GameObject iceCube;

    protected override void Awake()
    {
        startingSpeed = 2;

        base.Awake();

#if UNITY_WEBGL
        ClientManagerWeb.instance.Manager.Socket.On<string, string>("MethodCallToClient", MethodCalledFromServer);
#endif
    }

#if !UNITY_WEBGL
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Powerup"))
        {
    
        }
    }
#endif

    public Vector2 prevVector = Vector2.zero;
    protected override void CheckInput()
    {
#if UNITY_EDITOR
        if (isDebugPlayer && !isThrown)
        {
            Vector2 input = new Vector2(movement.x, movement.z);

            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector) * frictionCoefficient;
            Vector2 target = prevVector + delta;
            prevVector = target;

            collisionTimer = collisionTimerDefault;
        }
        else if(isDebugPlayer && isThrown)
        {
            // do not send input if held by VR player
        }
#endif
        if (IsLocal && !isThrown) //Only read values from analog stick, and emit movement if being done from local device
        {
            // in this game we want the player to feel like they're on ice, so we calculate a low fricition 
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector) * frictionCoefficient;
            Vector2 target = prevVector + delta;
            prevVector = target;

            if (!(target == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                ClientManagerWeb.instance.Manager.Socket.Emit("IS", target.x, target.y, PlayerByteID);
                Move(target.x, target.y);
            }

            collisionTimer = collisionTimerDefault;
        }
        else if (IsLocal && isThrown)
        {
            // do not send input if held by VR player
        }

        // check if we are below the floor
        if(transform.position.y < -10 && transform.position.y != posFromHost.y)
        {
            transform.position = posFromHost;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
        //anim.transform.rotation = Quaternion.RotateTowards(lookRotation, transform.rotation, Time.deltaTime);
    }

    protected override void Update()
    {
        // Make the player name track to the camera
#if UNITY_WEBGL
        playerNameText.transform.LookAt(2 * transform.position - cam.transform.position);
#else
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }
#endif
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // collide with objects
    }

#if UNITY_WEBGL
    void MethodCalledFromServer(string methodName, string data)
    {
        if (methodName.Equals("Gamers"))
        {
        }
    }
#endif

    // Jumping fields
    [SerializeField] private int jumpForce = 500;
    [SerializeField] int jumpCooldown = 2;
    float timeJumped = 0;
    public override void Action()
    {
        if (CanMove && timeJumped == 0 || (Time.time - timeJumped) > jumpCooldown)
        {
            timeJumped = Time.time;
            anim.SetTrigger("Jump");

            // Jump!
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }
}
