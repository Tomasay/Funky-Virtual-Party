using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Glitch9.AIDevKit.Demo
{
    public class DemoModelPrefab : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Text modelNameTxt;
        [SerializeField] private GameObject vision;
        [SerializeField] private GameObject legacy;

        private Image _image;
        public Model model
        {
            get => _model;
            set
            {
                _model = value;

                if (modelNameTxt == null)
                {
                    Debug.LogError("Model name text is not set");
                    return;
                }

                if (_model == null)
                {
                    Debug.LogError("Model is set to null");
                    return;
                }

                modelNameTxt.text = _model.Id;
                //vision.SetActive(_model.SupportsVision);
                legacy.SetActive(_model.IsLegacy);
            }
        }

        private Model _model;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected == value) return;
                if (_image == null) _image = GetComponent<Image>();
                _isSelected = value;
            }
        }

        private bool _isSelected;

        public event Action<Model> onSelect;

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Model clicked!: " + model.Id);
            onSelect?.Invoke(model);
        }

        private void OnEnable()
        {
            // MaterialThemeManager.Add(_image, ApplyThemeColor);
        }

        private void OnDisable()
        {
            // MaterialThemeManager.Remove(_image);
        }
    }
}