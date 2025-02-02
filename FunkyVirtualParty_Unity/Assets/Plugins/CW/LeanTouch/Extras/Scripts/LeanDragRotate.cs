using UnityEngine;
using CW.Common;

namespace Lean.Touch
{
    /// <summary>This component allows you to rotate the current GameObject around a specified axis using the finger drag gesture.</summary>
    [HelpURL(LeanTouch.HelpUrlPrefix + "LeanDragRotate")]
    [AddComponentMenu(LeanTouch.ComponentPathPrefix + "Drag Rotate")]
    public class LeanDragRotate : MonoBehaviour
    {
        /// <summary>The method used to find fingers to use with this component. See LeanFingerFilter documentation for more information.</summary>
        public LeanFingerFilter Use = new LeanFingerFilter(true);

        /// <summary>The rotation axis to use for this component.</summary>
        public Vector3 RotationAxis { set { rotationAxis = value; } get { return rotationAxis; } }
        [SerializeField] private Vector3 rotationAxis = Vector3.forward;

        /// <summary>The rotation speed will be multiplied by this.
        /// -1 = Inverted Controls.</summary>
        public float Sensitivity { set { sensitivity = value; } get { return sensitivity; } }
        [SerializeField] private float sensitivity = 1.0f;

        /// <summary>If you want this component to change smoothly over time, then this allows you to control how quick the changes reach their target value.
        /// -1 = Instantly change.
        /// 1 = Slowly change.
        /// 10 = Quickly change.</summary>
        public float Damping { set { damping = value; } get { return damping; } }
        [SerializeField] protected float damping = -1.0f;

        /// <summary>This allows you to control how much momentum is retained when the dragging fingers are all released.
        /// NOTE: This requires <b>Dampening</b> to be above 0.</summary>
        public float Inertia { set { inertia = value; } get { return inertia; } }
        [SerializeField] [Range(0.0f, 1.0f)] private float inertia;

        [SerializeField]
        private float remainingRotation;

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually add a finger.</summary>
        public void AddFinger(LeanFinger finger)
        {
            Use.AddFinger(finger);
        }

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove a finger.</summary>
        public void RemoveFinger(LeanFinger finger)
        {
            Use.RemoveFinger(finger);
        }

        /// <summary>If you've set Use to ManuallyAddedFingers, then you can call this method to manually remove all fingers.</summary>
        public void RemoveAllFingers()
        {
            Use.RemoveAllFingers();
        }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            Use.UpdateRequiredSelectable(gameObject);
        }
#endif

        protected virtual void Awake()
        {
            Use.UpdateRequiredSelectable(gameObject);
        }

        protected virtual void Update()
        {
            // Store old rotation
            var oldRotation = transform.localEulerAngles;

            // Get the fingers we want to use
            var fingers = Use.UpdateAndGetFingers();

            // Calculate the screenDelta value based on these fingers
            var screenDelta = LeanGesture.GetScreenDelta(fingers);

            if (screenDelta != Vector2.zero)
            {
                // Perform the rotation
                Rotate(screenDelta);
            }

            // Increment remaining rotation
            remainingRotation += Vector3.Dot(transform.localEulerAngles - oldRotation, rotationAxis);

            // Get damping factor
            var factor = CwHelper.DampenFactor(Damping, Time.deltaTime);

            // Dampen remaining rotation
            var newRemainingRotation = Mathf.Lerp(remainingRotation, 0.0f, factor);

            // Apply the dampened rotation
            transform.Rotate(rotationAxis, remainingRotation - newRemainingRotation);

            if (fingers.Count == 0 && inertia > 0.0f && Damping > 0.0f)
            {
                newRemainingRotation = Mathf.Lerp(newRemainingRotation, remainingRotation, inertia);
            }

            // Update remaining rotation with the dampened value
            remainingRotation = newRemainingRotation;
        }

        private void Rotate(Vector2 screenDelta)
        {
            // Calculate rotation angle based on screen delta
            var angle = screenDelta.x * Sensitivity;

            // Rotate the object around the specified axis
            transform.Rotate(rotationAxis, angle);
        }
    }
}
