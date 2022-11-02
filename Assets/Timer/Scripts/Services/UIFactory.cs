using Timer.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Timer.Scripts.Services
{
    public class UIFactory : IUIFactory
    {
        private const string UIRootPath = "UI/Root";
        private const string UITimeControllerPath = "UI/TimerController";
        private const string UITimerView = "UI/TimerView";
        private const string UINewTimerButtonPath = "UI/NewTimerButton";
        private const string UITimersContainer = "UI/TimersContainer";

        private readonly IProgressService _progressService;
        private readonly TweenController _tweenController;

        private RectTransform _uiRoot;

        public UIFactory(IProgressService progressService, TweenController tweenController)
        {
            _progressService = progressService;
            _tweenController = tweenController;
        }

        public UIRoot CreateUIRoot()
        {
            UIRoot rootPrefab = Resources.Load<UIRoot>(UIRootPath);
            var uiRoot = Object.Instantiate(rootPrefab);
            _uiRoot = uiRoot.UIRootTransform;

            return uiRoot;
        }

        public TimerContainer CreateTimerContainer(Vector2 spawnPosition, float containerSize)
        {
            TimerContainer timerContainerPrefab = Resources.Load<TimerContainer>(UITimersContainer);
            TimerContainer timerContainer = Object.Instantiate(timerContainerPrefab, _uiRoot);
            timerContainer.Construct(spawnPosition, containerSize);
            return timerContainer;
        }

        public TimerController CreateTimer(int ordinalNumber, Vector2 spawnPosition)
        {
            TimerController timerPrefab = Resources.Load<TimerController>(UITimeControllerPath);
            TimerController timer = Object.Instantiate(timerPrefab, _uiRoot);

            if (ordinalNumber == 0)
            {
                spawnPosition.x -= 0.55f * timer.GetComponent<RectTransform>().rect.width;
            }

            timer.GetComponent<RectTransform>().anchoredPosition = spawnPosition;

            timer.Construct(this, _tweenController, _uiRoot);
            timer.Init(_progressService.TimerSave.GetTimerData(ordinalNumber));

            return timer;
        }

        public TimerView CreateTimerView(TimerController timerController, Vector2 spawnPosition)
        {
            TimerView timerViewPrefab = Resources.Load<TimerView>(UITimerView);
            TimerView timerView = Object.Instantiate(timerViewPrefab, _uiRoot);

            RectTransform timerViewTransform = timerView.GetComponent<RectTransform>();
            spawnPosition.y += 0.55f * timerViewTransform.rect.height;
            timerViewTransform.anchoredPosition = spawnPosition;

            timerView.Construct(timerController);

            return timerView;
        }

        public RectTransform CreateNewTimerButton(Vector2 spawnPosition)
        {
            RectTransform newTimerButtonPrefab = Resources.Load<RectTransform>(UINewTimerButtonPath);
            RectTransform newTimerButton = Object.Instantiate(newTimerButtonPrefab, _uiRoot);
            newTimerButton.anchoredPosition = spawnPosition;

            return newTimerButton;
        }
    }
}