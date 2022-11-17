using System;
using System.Collections.Generic;
using Timer.Scripts.Data;
using Timer.Scripts.Services;
using Timer.Scripts.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Timer.Scripts
{
    public class CountdownTimer : MonoBehaviour, ICoroutineRunner
    {
        private const float YOffsetTimerPosition = 1.3f;
        private const float XOffsetTimerPosition = 0.55f;

        [Header("TimerDisplayOptions")] [SerializeField]
        private float _timerControllerMovingTime = 0.35f;

        [SerializeField] private float _moveInterval = 0.05f;

        private IProgressService _progressService;
        private ISaveLoadService _saveLoadService;
        private IUIFactory _factory;
        private TweenController _tweenController;
        private UIRoot _uiRoot;
        private TimerContainer _timersContainer;
        private RectTransform _newTimerButton;
        private LoadingCurtain _curtain;

        private List<TimerController> _timerControllers = new();
        private List<RectTransform> _objectsToMove = new();

        private bool _someTimerIsOpen;

        public void Construct(TweenController tweenController, IProgressService progressService,
            ISaveLoadService saveLoadService, IUIFactory factory, LoadingCurtain curtain, UIRoot uiRoot,
            TimerContainer timerContainer)
        {
            _tweenController = tweenController;
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _factory = factory;
            _curtain = curtain;
            _uiRoot = uiRoot;
            _timersContainer = timerContainer;
        }

        public void StartApp()
        {
            CreateTimers();
            SetContainerSpace();

            _curtain.Hide(MoveTimers);
        }

        private void SetContainerSpace() =>
            _timersContainer.SetVerticalLayoutSpacing(_objectsToMove[0].rect.height * 0.3f);

        private void CreateTimers()
        {
            int timerCount = _progressService.TimerSave.TimerData.Count;
            for (int i = 0; i < timerCount; i++)
            {
                CreateTimer(i);
            }

            CreateNewTimerButton();
        }

        private void CreateTimer(int id)
        {
            var spawnPosition = _uiRoot.StartPositionToFirstTimer;
            if (id != 0)
            {
                spawnPosition.x -= XOffsetTimerPosition * _objectsToMove[0].rect.width;
                spawnPosition.y -= YOffsetTimerPosition * _objectsToMove[0].rect.height * id;
            }

            TimerController timer = _factory.CreateTimer(id, spawnPosition);

            RegisterTimer(timer);
        }

        private void RegisterTimer(TimerController timer)
        {
            _timerControllers.Add(timer);
            _objectsToMove.Add(timer.GetComponent<RectTransform>());

            timer.OpenCloseTimer
                .Subscribe(OnOpenCloseTimer)
                .AddTo(timer);

            timer.SaveProgress
                .Subscribe(_ => OnSaveProgress())
                .AddTo(timer);
        }

        private void CreateNewTimerButton()
        {
            RectTransform lastTimerTransform = _objectsToMove[^1];
            Vector2 spawnPosition = lastTimerTransform.anchoredPosition;
            spawnPosition.y -= lastTimerTransform.rect.height * YOffsetTimerPosition;

            _newTimerButton = _factory.CreateNewTimerButton(spawnPosition);
            _newTimerButton.GetComponent<Button>().OnClickAsObservable()
                .Subscribe(_ => CreateNewTimer())
                .AddTo(this);
            _objectsToMove.Add(_newTimerButton);
        }

        private void CreateNewTimer()
        {
            int newTimerId = _timerControllers.Count;
            _progressService.TimerSave.TimerData.Add(new TimerData(newTimerId));

            SetParent(_newTimerButton, _uiRoot.UIRootTransform);
            Vector2 spawnPosition = _newTimerButton.anchoredPosition;
            TimerController timer = _factory.CreateTimer(newTimerId, spawnPosition);

            _objectsToMove.Remove(_newTimerButton);

            RegisterTimer(timer);
            SetParent(timer.GetComponent<RectTransform>(), _timersContainer.TimerContainerTransform);
            ReplaceNewTimerButton();
            OnSaveProgress();
        }

        private void ReplaceNewTimerButton()
        {
            _newTimerButton.anchoredPosition -= new Vector2(0, _newTimerButton.rect.height * YOffsetTimerPosition);
            _objectsToMove.Add(_newTimerButton);
            SetParent(_newTimerButton, _timersContainer.TimerContainerTransform);
        }

        private void OnSaveProgress() =>
            _saveLoadService.Save();

        private void OnOpenCloseTimer(bool isOpen)
        {
            foreach (TimerController timerController in _timerControllers)
            {
                timerController.TimerIsOpen = isOpen;
            }

            Vector2 endPosition = new Vector2(
                _uiRoot.StartPositionToFirstTimer.x - XOffsetTimerPosition * _objectsToMove[0].rect.width,
                _uiRoot.StartPositionToFirstTimer.y);

            Action onComplete = null;
            if (isOpen)
            {
                _timersContainer.SetScroll(0);
                RemoveTimersParent();
                ResetAnchorsAndPosition();
            }

            if (!isOpen)
            {
                endPosition = new Vector2(0, 0.25f * _uiRoot.GetHeight());
                onComplete = SetTimersParent;
            }

            _tweenController.MovementObject(_objectsToMove, endPosition,
                _timerControllerMovingTime, _moveInterval, onComplete);
        }

        private void MoveTimers()
        {
            _tweenController.MovementObject(_objectsToMove, new Vector2(0, 0.25f * _uiRoot.GetHeight()),
                _timerControllerMovingTime, _moveInterval, SetTimersParent);
        }

        private void ResetAnchorsAndPosition()
        {
            Vector2 position = new Vector2(0, 0.25f * _uiRoot.GetHeight());
            for (var i = 0; i < _objectsToMove.Count; i++)
            {
                _objectsToMove[i].anchorMax = new Vector2(0.5f, 0.5f);
                _objectsToMove[i].anchorMin = new Vector2(0.5f, 0.5f);
                if (i != 0)
                {
                    position.y -= _objectsToMove[i].rect.height * YOffsetTimerPosition;
                }

                _objectsToMove[i].anchoredPosition = position;
            }
        }

        private void SetTimersParent()
        {
            foreach (RectTransform rectTransform in _objectsToMove)
            {
                SetParent(rectTransform, _timersContainer.TimerContainerTransform);
            }
        }

        private void RemoveTimersParent()
        {
            foreach (RectTransform rectTransform in _objectsToMove)
            {
                SetParent(rectTransform, _uiRoot.UIRootTransform);
            }
        }

        private void SetParent(RectTransform rectTransform, RectTransform parent) =>
            rectTransform.SetParent(parent);
    }
}