using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.OpenAI;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoThreadPrefab : MonoBehaviour, IPointerClickHandler
    {
        [NonSerialized] public UnityEngine.UI.Image image;
        public Thread Thread { get; private set; }
        public string ThreadId { get; private set; }

        private Text _threadIdTxt;
        private DemoThreadContainer _threadContainer;

        public bool IsSelected { get; set; }


        private void OnEnable()
        {
            image = GetComponent<UnityEngine.UI.Image>();
        }

        public void Initialize(DemoThreadContainer threadContainer, string threadId)
        {
            ThreadId = threadId;
            _threadContainer = threadContainer;
            _threadIdTxt = GetComponentInChildren<Text>();
            _threadIdTxt.text = threadId;
        }

        public void SetThread(Thread thread)
        {
            Thread = thread;
            _threadIdTxt.text = thread.Id;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnPointerClickAsync().Forget();
        }

        private async UniTask OnPointerClickAsync()
        {
            if (Thread == null && !string.IsNullOrEmpty(ThreadId))
            {
                Thread = await _threadContainer.Api.Controller.SetThreadAsync(ThreadId);
            }
            else
            {
                _threadContainer.SelectThread(Thread);
            }
        }
    }
}