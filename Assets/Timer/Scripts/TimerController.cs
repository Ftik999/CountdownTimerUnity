using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts
{
    [RequireComponent(typeof(Image))]
    public class TimerController : MonoBehaviour
    {
        public event Action<string> ChangeDisplayValue;
        public event Action<bool> OpenCloseTimer;

        private const float PressedValueCoefficient = 0.4f;
        private const float TimeDelay = 0.7f;
        private const int ValueForChange = 5;
        private const int SecondsOnDay = 86400;

        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Button _openTimer;

        [Header("Animation Options")] [SerializeField]
        private float _changeColorAnimationTime = 3f;

        [SerializeField] private float _shakeIntensity = 1;
        [SerializeField] private float _timerMovingTime = 0.35f;
        [SerializeField] private Color CompleteTimerColor = Color.yellow;

        private UIFactory _factory;
        private TweenController _tweenController;
        private RectTransform _uiRoot;
        private TimerView _timerView;

        private bool _isStarted;
        private bool _isComplete;
        private double _currentTimer;


        //Pressed Button options
        private bool _addButtonIsActive;
        private bool _removeButtonIsActive;
        private float _buttonPressedTime;


        //end Timer animation
        private Image _image;
        private Coroutine _colorCoroutine;
        private Coroutine _shakeCoroutine;
        private Color _defaultColor;

        public bool TimerIsOpen { get; set; }

        private void Start()
        {
            _image = GetComponent<Image>();
            _defaultColor = _image.color;

            _openTimer.onClick.AddListener(OpenTimer);
        }

        private void Update()
        {
            if (_addButtonIsActive || _removeButtonIsActive)
            {
                ChangeTimerValueWhenButtonIsPressed();
            }

            TimerLogic();
        }

        public void Construct(UIFactory factory, TweenController tweenController, RectTransform uiRoot,
            int ordinalNumber)
        {
            _uiRoot = uiRoot;
            _factory = factory;
            _tweenController = tweenController;
            _buttonText.text = "Timer " + ordinalNumber;
        }

        private void ChangeTimerValueWhenButtonIsPressed()
        {
            if (_buttonPressedTime > TimeDelay)
            {
                if (_addButtonIsActive)
                {
                    AddTime(PressedValueCoefficient * _buttonPressedTime);
                }

                if (_removeButtonIsActive)
                {
                    RemoveTime(PressedValueCoefficient * _buttonPressedTime);
                }
            }

            _buttonPressedTime += Time.deltaTime;
        }

        private void TimerLogic()
        {
            if (!_isStarted)
            {
                return;
            }

            if (_currentTimer > 0)
            {
                _currentTimer -= Time.deltaTime;
                DisplayTimer();
            }
            else
            {
                Stop();
            }
        }

        private void Stop()
        {
            _isStarted = false;
            _isComplete = true;
            _currentTimer = 0;

            if (!TimerIsOpen)
            {
                StopEndTimerCoroutine();

                _colorCoroutine = _tweenController.ChangeColor(_image, CompleteTimerColor, _changeColorAnimationTime);
                _shakeCoroutine = _tweenController.ShakeObject(transform, _shakeIntensity);
            }
            else
            {
                _image.color = CompleteTimerColor;
            }
        }

        private void OpenTimer()
        {
            if (TimerIsOpen)
                return;

            StopEndTimerCoroutine();

            OpenCloseTimer?.Invoke(true);

            CreateTimerView();
            MoveTimerView(Vector2.zero);
            DisplayTimer();
        }

        private void ResetRotation() =>
            transform.rotation = Quaternion.identity;

        private void StopEndTimerCoroutine()
        {
            if (_colorCoroutine != null || _shakeCoroutine != null)
            {
                ResetRotation();
                _tweenController.StopCoroutine(_colorCoroutine);
                _tweenController.StopCoroutine(_shakeCoroutine);
                _colorCoroutine = null;
                _shakeCoroutine = null;
            }
        }

        private void CreateTimerView()
        {
            _timerView = _factory.CreateTimerView(this, new Vector2(0, _uiRoot.rect.height / 2));

            _timerView.AddTimerValue += AddTimerValue;
            _timerView.RemoveTimerValue += RemoveTimeValue;
            _timerView.StartTimer += StartTimer;
        }

        private void StartTimer()
        {
            if (_currentTimer != 0)
            {
                _isStarted = true;
            }

            OpenCloseTimer?.Invoke(false);

            MoveTimerView(new Vector2(0, -_uiRoot.rect.height), DestroyTimerView);

            if (_isComplete)
            {
                _image.color = _defaultColor;
                _isComplete = false;
            }
        }

        private void DestroyTimerView()
        {
            Destroy(_timerView.gameObject);
            _timerView = null;
        }

        private void AddTimerValue(bool isPressed)
        {
            _addButtonIsActive = isPressed;

            if (_addButtonIsActive)
            {
                _buttonPressedTime = 0f;
                AddTime(ValueForChange);
            }
        }

        private void RemoveTimeValue(bool isPressed)
        {
            _removeButtonIsActive = isPressed;

            if (_removeButtonIsActive)
            {
                _buttonPressedTime = 0f;
                RemoveTime(ValueForChange);
            }
        }

        private void AddTime(float value)
        {
            double newTime = _currentTimer + value;

            if (newTime > SecondsOnDay)
            {
                ChangeTimerValue(_isStarted ? SecondsOnDay : newTime - SecondsOnDay);
            }
            else
            {
                ChangeTimerValue(newTime);
            }

            DisplayTimer();
        }

        private void RemoveTime(float value)
        {
            double newTime = _currentTimer - value;

            if (newTime < 0)
            {
                ChangeTimerValue(SecondsOnDay - newTime * -1);
            }
            else
            {
                ChangeTimerValue(newTime);
            }

            DisplayTimer();
        }

        private string ConvertSecondsToStandardTime(double seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            string timer = timeSpan.Hours + " : " +
                           timeSpan.Minutes.ToString("D2") + " : " +
                           timeSpan.Seconds.ToString("D2");

            return timer;
        }

        private void MoveTimerView(Vector2 endPosition, Action onComplete = null) =>
            _tweenController.MovementObject(_timerView.GetComponent<RectTransform>(),
                endPosition, _timerMovingTime, onComplete);

        private void ChangeTimerValue(double newTime) =>
            _currentTimer = newTime;

        private void DisplayTimer() =>
            ChangeDisplayValue?.Invoke(ConvertSecondsToStandardTime(_currentTimer));
    }
}