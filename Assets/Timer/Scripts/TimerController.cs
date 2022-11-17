using System;
using Timer.Scripts.Data;
using Timer.Scripts.Services;
using Timer.Scripts.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts
{
    [RequireComponent(typeof(Image))]
    public class TimerController : MonoBehaviour
    {
        public ReactiveCommand SaveProgress = new ReactiveCommand();
        public ReactiveCommand<bool> OpenCloseTimer = new ReactiveCommand<bool>();

        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Button _openTimer;

        [Header("Animation Options")] 
        [SerializeField] private float _changeColorAnimationTime = 3f;
        [SerializeField] private float _shakeIntensity = 1;
        [SerializeField] private float _timerMovingTime = 0.35f;
        [SerializeField] private Color _completeTimerColor = Color.yellow;

        private IUIFactory _factory;
        private TweenController _tweenController;
        private RectTransform _uiRoot;
        private TimerView _timerView;
        private Timer _timer;

        //end Timer animation
        private Image _image;
        private Coroutine _colorCoroutine;
        private Coroutine _shakeCoroutine;
        private Color _defaultColor;

        public bool TimerIsOpen { get; set; } = true;

        public void Construct(UIFactory factory, TweenController tweenController, RectTransform uiRoot)
        {
            _uiRoot = uiRoot;
            _factory = factory;
            _tweenController = tweenController;

            _image = GetComponent<Image>();
            _defaultColor = _image.color;
            _openTimer.OnClickAsObservable()
                .Subscribe(_ => OpenTimer())
                .AddTo(this);
        }

        private void TimerValueChanged(double timerTime)
        {
            if (_timerView != null)
            {
                _timerView.DisplayTimer(ConvertSecondsToStandardTime(timerTime));
            }
        }

        public void Init(TimerData timerData, Updater updater)
        {
            _timer = new Timer(timerData, updater);
            _timer.TimerTime
                .ObserveEveryValueChanged(x => x.Value)
                .Subscribe(TimerValueChanged)
                .AddTo(this);

            _timer.TimerStopped
                .Subscribe(_ => OnTimerStopped())
                .AddTo(this);

            if (_timer.IsComplete)
            {
                RenderTimerButtonChanges();
            }

            TimerIsOpen = false;
            _buttonText.text = "Timer " + (timerData.Id + 1);
        }

        private void OnTimerStopped()
        {
            Save();
            RenderTimerButtonChanges();
        }

        private void RenderTimerButtonChanges()
        {
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
        }

        private void OpenTimer()
        {
            if (TimerIsOpen)
                return;

            StopTimerEndCoroutine();

            OpenCloseTimer?.Execute(true);

            CreateTimerView();
            MoveTimerView(Vector2.zero);
            _timer.TimerReviewed();
            TimerValueChanged(_timer.TimerTime.Value);
            
            Save();
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

            _timerView.PlusButton.ButtonState
                .ObserveEveryValueChanged(isPressed => isPressed.Value)
                .Subscribe(AddTimerValue)
                .AddTo(_timerView);

            _timerView.MinusButton.ButtonState
                .ObserveEveryValueChanged(isPressed => isPressed.Value)
                .Subscribe(RemoveTimeValue)
                .AddTo(_timerView);

            _timerView.StartButton.OnClickAsObservable()
                .Subscribe(_ => StartTimer())
                .AddTo(_timerView);
        }

        private void DestroyTimerView()
        {
            Destroy(_timerView.gameObject);
            _timerView = null;
        }

        private void StartTimer()
        {
            _timer.StartTimer();
            _image.color = _defaultColor;

            CloseTimerView();
            ShowAllTimers();
            Save();
        }

        private void ShowAllTimers() =>
            OpenCloseTimer?.Execute(false);
        
        private void CloseTimerView() =>
            MoveTimerView(new Vector2(0, -_uiRoot.rect.height), DestroyTimerView);

        private void AddTimerValue(bool isPressed) => 
            _timer.AddTimerValue(isPressed);

        private void RemoveTimeValue(bool isPressed) => 
            _timer.RemoveTimeValue(isPressed);

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

        private void Save() =>
            SaveProgress?.Execute();
    }
}