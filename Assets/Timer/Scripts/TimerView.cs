using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts
{
    public class TimerView : MonoBehaviour
    {
        public event Action<bool> AddTimerValue;
        public event Action<bool> RemoveTimerValue;
        public event Action StartTimer;

        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private CustomButton _plusButton;
        [SerializeField] private CustomButton _minusButton;
        [SerializeField] private Button _startButton;

        private void Start()
        {
            _plusButton.ChangeButtonState += OnChangePlusButtonState;
            _minusButton.ChangeButtonState += OnChangeMinusButtonState;
            _startButton.onClick.AddListener(PressStart);
        }

        public void Construct(TimerController timerController) =>
            timerController.ChangeDisplayValue += DisplayTimer;

        private void OnChangePlusButtonState(bool isPressed) =>
            AddTimerValue?.Invoke(isPressed);

        private void OnChangeMinusButtonState(bool isPressed) =>
            RemoveTimerValue?.Invoke(isPressed);

        private void PressStart() =>
            StartTimer?.Invoke();

        private void DisplayTimer(string timerText) =>
            _timerText.text = timerText;
    }
}