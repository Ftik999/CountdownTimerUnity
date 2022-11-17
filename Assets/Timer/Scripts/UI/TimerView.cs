using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts.UI
{
    public class TimerView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _timerText;
        [SerializeField] private CustomButton _plusButton;
        [SerializeField] private CustomButton _minusButton;
        [SerializeField] private Button _startButton;

        public CustomButton PlusButton => _plusButton;
        public CustomButton MinusButton => _minusButton;
        public Button StartButton => _startButton;

        public void DisplayTimer(string timerText) =>
            _timerText.text = timerText;
    }
}