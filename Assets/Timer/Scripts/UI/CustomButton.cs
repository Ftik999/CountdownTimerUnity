using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Timer.Scripts.UI
{
    [RequireComponent(typeof(Image))]
    public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private Image _image;
        private Color _defaultColor;
        private Color _selectedColor = new(0.85f, 0.85f, 0.85f, 1);

        public ReactiveProperty<bool> ButtonState = new();

        private void Start()
        {
            _image = GetComponent<Image>();
            _defaultColor = _image.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            ButtonState.Value = true;
            _image.color = _selectedColor;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ButtonState.Value = false;
            _image.color = _defaultColor;
        }
    }
}