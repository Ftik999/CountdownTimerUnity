using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timer.Scripts
{
    public class CountdownTimer : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _curtain;
        [SerializeField] private float _timerControllerMovingTime = 0.35f;
        [SerializeField] private float _moveInterval = 0.05f;

        private UIFactory _factory;
        private RectTransform _uiRoot;
        private TweenController _tweenController;

        private List<TimerController> _timeControllers = new();
        private Vector2 _startPositionToFirstTimer;

        private bool _someTimerIsOpen;

        private void Awake()
        {
            StartApp();
        }

        private void StartApp()
        {
            _tweenController = new TweenController(this);
            
            _factory = new UIFactory(_tweenController);
            
            _uiRoot = _factory.CreateUIRoot();
            float canvasWidth = _uiRoot.rect.width;
            float canvasHeight = _uiRoot.rect.height;
            _startPositionToFirstTimer = new Vector2(-canvasWidth / 2, 0.25f * canvasHeight);

            for (int i = 0; i < 3; i++)
            {
                _timeControllers.Add(_factory.CreateTimer(i + 1, _startPositionToFirstTimer));
                _timeControllers[i].OpenCloseTimer += OnOpenCloseTimer;
            }
            
            _curtain.Hide(MoveTimers);
        }

        private void OnOpenCloseTimer(bool isOpen)
        {
            foreach (TimerController timerController in _timeControllers)
            {
                timerController.TimerIsOpen = isOpen;
            }

            Vector2 endPosition = new Vector2(
                _startPositionToFirstTimer.x - 0.55f * _timeControllers[0].GetComponent<RectTransform>().rect.width,
                _startPositionToFirstTimer.y);

            if (!isOpen)
            {
                endPosition = new Vector2(0, 0.25f * _uiRoot.rect.height);
            }

            _tweenController.MovementObject(_timeControllers.Select(x => x.GetComponent<RectTransform>()).ToArray(),
                endPosition, _timerControllerMovingTime, _moveInterval, null);
        }

        private void MoveTimers()
        {
            _tweenController.MovementObject(
                _timeControllers.Select(x => x.GetComponent<RectTransform>()).ToArray(),
                new Vector2(0, 0.25f * _uiRoot.rect.height), _timerControllerMovingTime, _moveInterval, null);
        }
    }
}