using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoModelSelector : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text modelNameTxt;
        [SerializeField] private DemoModelSelectorPopup popup;
        [SerializeField] private string modelFamily;

        public bool isVisible => popupObject.activeSelf;
        public Model selected { get; set; }

        private GameObject popupObject;

        public event Action<Model> onModelChanged;

        private void Awake()
        {
            popupObject = popup.gameObject;
        }

        public void SetFamily(string family)
        {
            modelFamily = family;
        }

        public void Show()
        {
            popupObject.SetActive(true);
            popup.Show(this, modelFamily);
        }

        public void Hide()
        {
            popupObject.SetActive(false);
        }

        public void Select(Model model)
        {
            if (model.Family != modelFamily) return;
            selected = model;
            onModelChanged?.Invoke(model);
            if (modelNameTxt != null) modelNameTxt.text = model.Id;
            Hide();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isVisible) return;
            Show();
        }
    }
}