using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;
using Autohand;

#if UNITY_WEBGL
using System.Runtime.InteropServices;
#endif
#if UNITY_EDITOR
using UnityEngine.InputSystem;
#endif

public class KaijuGameClientPlayer : ClientPlayer
{
    public enum KaijuClientState
    {
        OnGround,
        Jump,
        Grabbed,
        Thrown
    }
    public KaijuClientState state = KaijuClientState.OnGround;
    
    const float collisionTimerDefault = 1;
    Vector2 collisionVector;


    public Camera cam;

    private RigidbodyConstraints currentConstraints;

    protected override void Awake()
    {
        startingSpeed = 4;
#if UNITY_EDITOR
        canMove = true;
#endif
        base.Awake();
    }

#if !UNITY_WEBGL
    private void OnTriggerEnter(Collider other)
    {

    }
#endif

    public Vector2 prevVector = Vector2.zero;
    protected override void CheckInput()
    {
#if UNITY_EDITOR
        if (syncer.IsDebugPlayer && state < KaijuClientState.Grabbed)
        {
            Vector2 input = new Vector2(movement.x, movement.z);

            if (prevVector == Vector2.zero)
                prevVector = input;

            Vector2 delta = (input - prevVector);
            Vector2 target = prevVector + delta;
            prevVector = target;

            
        }
        else if(syncer.IsDebugPlayer)
        {
            // do not send input if held by VR player
        }

        if (true)
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            //Input should be relative to camera, which is always facing forward from the player
            if (input.magnitude > 0.1f)
            {
                float magnitude = input.magnitude;
                input = input.normalized;
                float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y; // + camera eulerAngles y
                Vector3 newInput = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                input = new Vector2(newInput.x, newInput.z) * magnitude;
            }

            if (!(input == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                //ClientManagerWeb.instance.Manager.Socket.Emit("IS", SerializeInputData(input));
                Move(input);
            }
        }
#endif

        if (IsLocal && state != KaijuClientState.Grabbed) //Only read values from analog stick, and emit movement if being done from local device
        {
            Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();

            //Input should be relative to camera, which is always facing forward from the player
            if (input.magnitude > 0.1f)
            {
                float magnitude = input.magnitude;
                input = input.normalized;
                float targetAngle = Mathf.Atan2(input.x, input.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y; // + camera eulerAngles y
                Vector3 newInput = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
                input = new Vector2(newInput.x, newInput.z) * magnitude;
            }

            if (!(input == Vector2.zero && movement == Vector3.zero)) //No need to send input if we're sending 0 and we're already not moving
            {
                //ClientManagerWeb.instance.Manager.Socket.Emit("IS", SerializeInputData(input));
                Move(input);
            }

        }
        else if (IsLocal)
        {
            // do not send input if held by VR player
        }
    }

    protected override void Update()
    {
        // Make the player name track to the camera
#if UNITY_WEBGL
        //playerNameText.transform.LookAt(2 * transform.position - cam.transform.position);
        CheckInput();
#else
        if (Camera.main)
        {
            playerNameText.transform.LookAt(2 * transform.position - Camera.main.transform.position);
        }
#endif

#if UNITY_EDITOR
        if(Keyboard.current.zKey.wasPressedThisFrame)
        {
            state = KaijuClientState.Thrown;
            anim.SetBool("Flying", true);
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 45, gameObject.transform.position.z);
        }
#endif
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        // collide with objects
        if (collision.collider.CompareTag("Untagged"))
        {
            anim.SetBool("Flying", false);
            state = KaijuClientState.OnGround;
        }

    }

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
            state = KaijuClientState.Jump;
        }
    }

    public void OnGrabbed(Hand h, Grabbable g)
    {
        currentConstraints = rb.constraints;
        rb.constraints = RigidbodyConstraints.None;

        anim.transform.localRotation = Quaternion.identity;

        anim.SetBool("Grabbed", true);
        state = KaijuClientState.Grabbed;
        CanMove = false;
    }

    public void OnDropped(Hand h, Grabbable g)
    {
        rb.constraints = currentConstraints;

        anim.SetBool("Grabbed", false);
        state = KaijuClientState.Thrown;
        anim.SetBool("Flying", true);
#if UNITY_WEBGL
        realtimeView.RequestOwnership();
        realtimeTransform.RequestOwnership();
#endif

        CanMove = true;

    }
}
