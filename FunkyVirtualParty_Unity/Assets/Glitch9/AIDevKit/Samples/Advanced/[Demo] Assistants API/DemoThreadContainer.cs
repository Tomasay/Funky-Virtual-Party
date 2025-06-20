using Cysharp.Threading.Tasks;
using Glitch9.AIDevKit.Components;
using Glitch9.AIDevKit.OpenAI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoThreadContainer : MonoBehaviour
    {
        [SerializeField] private DemoThreadPrefab threadPrefabPrefab;
        [SerializeField] private Transform threadButtonContainer;
        [SerializeField] private DemoButton startNewThreadButton;

        public AssistantChatbot Api { get; private set; }
        public Thread CurrentThread { get; set; }

        public event Action<Thread> onThreadSelect;
        private readonly List<DemoThreadPrefab> _threadButtons = new();


        private void Start()
        {
            // Null check all inspector fields
            if (threadPrefabPrefab == null || threadButtonContainer == null || startNewThreadButton == null)
            {
                Debug.LogError("Inspector fields are not set");
            }

            startNewThreadButton.onClick += StartNewThread;
        }

        public void Initialize(AssistantChatbot chatGPT, List<string> threadIds)
        {
            Api = chatGPT;
            ResetThreads(threadIds);
        }

        public void StartNewThread()
        {
            Api.Controller.CreateThreadAsync().Forget();
        }

        public void ResetThreads(List<string> threadIds)
        {
            _threadButtons.Clear();

            foreach (Transform child in threadButtonContainer)
            {
                Destroy(child.gameObject);
            }

            foreach (string threadId in threadIds)
            {
                DemoThreadPrefab threadPrefab = Instantiate(threadPrefabPrefab, threadButtonContainer);
                threadPrefab.Initialize(this, threadId);
                _threadButtons.Add(threadPrefab);
            }
        }

        public void OnThreadCreated(Thread thread)
        {
            if (thread == null)
            {
                Debug.LogError("Thread is null");
                return;
            }

            DemoThreadPrefab threadPrefab = Instantiate(threadPrefabPrefab, threadButtonContainer);
            threadPrefab.Initialize(this, thread.Id);
            _threadButtons.Add(threadPrefab);

            SelectThread(thread);
        }

        public void SelectThread(Thread thread)
        {
            if (thread == null)
            {
                Debug.LogError("Thread is null");
                return;
            }

            CurrentThread = thread;
            onThreadSelect?.Invoke(thread);

            foreach (DemoThreadPrefab threadButton in _threadButtons)
            {
                threadButton.IsSelected = threadButton.ThreadId == thread.Id;
            }
        }
    }
}