using UnityEngine;
using UnityEngine.Events;
#if !UNITY_WEBGL
using FMODUnity;
#endif
using DG.Tweening;

namespace Autohand{
    //THIS MAY NOT WORK AS A GRABBABLE AT THIS TIME - Try PhysicsGadgetSlider instead
    public class PhysicsGadgetButton : PhysicsGadgetConfigurableLimitReader{
        bool pressed = false;

        [Tooltip("The percentage (0-1) from the required value needed to call the event, if threshold is 0.1 OnPressed will be called at 0.9, and OnUnpressed at 0.1"), Min(0.01f)]
        public float threshold = 0.1f;
        public float pressedThreshold, unPressedThreshold;
        public bool lockOnPressed = false;
        [Space]
        public UnityEvent OnPressed;
        public UnityEvent OnUnpressed;

        Vector3 startPos;
        Vector3 pressedPos;
        float pressedValue;

        float timePressed, timeUnpressed;

        new protected void Start(){
            base.Start();
            startPos = transform.localPosition;
        }

        public void OnDistancePress()
        {
            transform.DOLocalMoveY(-(pressedThreshold / 10), 0.25f);
            //transform.localPosition = new Vector3(transform.position.x, -0.25f, transform.position.z);
            Pressed();
        }

        protected void FixedUpdate(){
            var value = GetValue();

            /*
            if(!pressed && value+threshold >= 1) {
                Pressed();
            }
            else if(!lockOnPressed && pressed && value-threshold <= 0){
                Unpressed();
            }
            */

            if (!pressed && value >= pressedThreshold)
            {
                Pressed();
            }
            else if (!lockOnPressed && pressed && value <= unPressedThreshold)
            {
                Unpressed();
            }

            if (value < 0)
                transform.localPosition = startPos;

            if (pressed && lockOnPressed && value + threshold < pressedValue)
                transform.localPosition = pressedPos;
        }


        public void Pressed() {
#if !UNITY_WEBGL
            RuntimeManager.PlayOneShot("event:/SFX/ButtonDown", transform.position);
#endif
            pressed = true;
            pressedValue = GetValue();
            pressedPos = transform.localPosition;
            OnPressed?.Invoke();
        }

        public void Unpressed(){
#if !UNITY_WEBGL
            RuntimeManager.PlayOneShot("event:/SFX/ButtonUp", transform.position);
#endif
            pressed = false;
            OnUnpressed?.Invoke();
        }

        public void Unlock() {
            lockOnPressed = false;
            GetComponent<Rigidbody>().WakeUp();
        }
    }
}
