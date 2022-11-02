using System;
using Timer.Scripts.Data;
using Timer.Scripts.Services;
using Timer.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts
{
    [RequireComponent(typeof(Image))]
    public class TimerController : MonoBehaviour
    {
        public event Action SaveProgress;
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
        [SerializeField] private Color _completeTimerColor = Color.yellow;

        private IUIFactory _factory;
        private TweenController _tweenController;
        private RectTransform _uiRoot;
        private TimerData _timerData;
        private TimerView _timerView;

        private bool _isStarted;
        private bool _isComplete;
        private double _currentTimer;
        private long _startTimeToUnix;
        private int _id;


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

        private void OnDestroy() =>
            _openTimer.onClick.RemoveListener(OpenTimer);

        private void Update()
        {
            if (_addButtonIsActive || _removeButtonIsActive)
            {
                ChangeTimerValueWhenButtonIsPressed();
            }

            TimerLogic();
        }

        public void Construct(UIFactory factory, TweenController tweenController, RectTransform uiRoot)
        {
            _uiRoot = uiRoot;
            _factory = factory;
            _tweenController = tweenController;

            _image = GetComponent<Image>();
            _defaultColor = _image.color;
            _openTimer.onClick.AddListener(OpenTimer);
        }

        public void Init(TimerData timerData)
        {
            _timerData = timerData;
            _id = timerData.Id;
            _currentTimer = timerData.TimerTime;
            _startTimeToUnix = timerData.StartTimeToUnix;
            _isComplete = timerData.IsComplete;
            _buttonText.text = "Timer " + (_id + 1);

            InitTimer();
        }

        private void InitTimer()
        {
            if (_currentTimer > 0)
            {
                var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (_startTimeToUnix + _currentTimer < currentUnixTime)
                {
                    _currentTimer = 0;
                    _image.color = _completeTimerColor;
                    _isComplete = true;
                }
                else
                {
                    _currentTimer = _startTimeToUnix + _currentTimer - currentUnixTime;
                    _isStarted = true;
                }
            }
            else
            {
                if (_isComplete)
                {
                    _image.color = _completeTimerColor;
                }
            }
        }

        private void ChangeTimerValueWhenButtonIsPressed()
        {
            if (_buttonPressedTime > TimeDelay)
            {
                if (_addButtonIsActive)
                {
                    IncreaseTime(PressedValueCoefficient * _buttonPressedTime);
                }

                if (_removeButtonIsActive)
                {
                    DecreaseTime(PressedValueCoefficient * _buttonPressedTime);
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
                StopTimerEndCoroutine();

                _colorCoroutine = _tweenController.ChangeColor(_image, _completeTimerColor, _changeColorAnimationTime);
                _shakeCoroutine = _tweenController.ShakeObject(transform, _shakeIntensity);
            }
            else
            {
                _image.color = _completeTimerColor;
            }

            WriteStopTimerSaves();
        }

        private void OpenTimer()
        {
            if (TimerIsOpen)
                return;

            StopTimerEndCoroutine();

            OpenCloseTimer?.Invoke(true);

            CreateTimerView();
            MoveTimerView(Vector2.zero);
            DisplayTimer();

            WriteCompleteSaves();
        }

        private void ResetRotation() =>
            transform.rotation = Quaternion.identity;

        private void StopTimerEndCoroutine()
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
            if (_currentTimer > 0)
            {
                _isStarted = true;
                _startTimeToUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                WriteStartTimerSaves();
            }
            else
            {
                WriteCompleteSaves();
            }

            _image.color = _defaultColor;
            _isComplete = false;

            CloseTimerView();
            ShowAllTimers();
        }

        private void ShowAllTimers() =>
            OpenCloseTimer?.Invoke(false);

        private void CloseTimerView() =>
            MoveTimerView(new Vector2(0, -_uiRoot.rect.height), DestroyTimerView);

        private void DestroyTimerView()
        {
            _timerView.AddTimerValue -= AddTimerValue;
            _timerView.RemoveTimerValue -= RemoveTimeValue;
            _timerView.StartTimer -= StartTimer;

            Destroy(_timerView.gameObject);
            _timerView = null;
        }

        private void AddTimerValue(bool isPressed)
        {
            _addButtonIsActive = isPressed;

            if (_addButtonIsActive)
            {
                _buttonPressedTime = 0f;
                IncreaseTime(ValueForChange);
            }
        }

        private void RemoveTimeValue(bool isPressed)
        {
            _removeButtonIsActive = isPressed;

            if (_removeButtonIsActive)
            {
                _buttonPressedTime = 0f;
                DecreaseTime(ValueForChange);
            }
        }

        private void IncreaseTime(float value)
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

        private void DecreaseTime(float value)
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

        private void Save() =>
            SaveProgress?.Invoke();

        private void WriteStopTimerSaves()
        {
            _timerData.IsComplete = true;
            _timerData.StartTimeToUnix = 0;
            _timerData.TimerTime = 0;

            Save();
        }

        private void WriteCompleteSaves()
        {
            if (_isComplete)
            {
                _timerData.IsComplete = false;
                _timerData.TimerTime = 0;

                Save();
            }
        }

        private void WriteStartTimerSaves()
        {
            _timerData.IsComplete = false;
            _timerData.TimerTime = _currentTimer;
            _timerData.StartTimeToUnix = _startTimeToUnix;
            Save();
        }
    }
}