using Glitch9.AIDevKit.OpenAI;
using UnityEngine;
using UnityEngine.Events;

namespace Glitch9.AIDevKit.Components
{
    [AddComponentMenu("Event Receiver (Run & RunStep)")]
    public class RunEventReceiver : MonoBehaviour, IRunEventReceiver
    {
        [SerializeField] private UnityEvent<Run> onRunCreated;
        [SerializeField] private UnityEvent<Run> onRunRetrieved;
        [SerializeField] private UnityEvent<Run> onRunUpdated;
        [SerializeField] private UnityEvent<RunStep> onRunStepRetrieved;
        [SerializeField] private UnityEvent<RunStatus> onRunStatusChanged;

        public void OnRunCreated(Run run) => onRunCreated?.Invoke(run);
        public void OnRunRetrieved(Run run) => onRunRetrieved?.Invoke(run);
        public void OnRunStatusChanged(RunStatus runStatus) => onRunStatusChanged?.Invoke(runStatus);
        public void OnRunStepRetrieved(RunStep runStep) => onRunStepRetrieved?.Invoke(runStep);
        public void OnRunUpdated(Run run) => onRunUpdated?.Invoke(run);
    }
}