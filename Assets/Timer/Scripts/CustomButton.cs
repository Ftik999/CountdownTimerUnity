using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Timer.Scripts
{
    [RequireComponent(typeof(Image))]
    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public event Action<bool> ChangeButtonState;

        private Image _image;
        private Color _defaultColor;
        private Color _selectedColor = new(0.85f, 0.85f, 0.85f, 1);

        private void Start()
        {
            _image = GetComponent<Image>();
            _defaultColor = _image.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ChangeButtonState?.Invoke(true);
            _image.color = _selectedColor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ChangeButtonState?.Invoke(false);
            _image.color = _defaultColor;
        }
    }
}