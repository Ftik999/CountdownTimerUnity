using Timer.Scripts.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Timer.Scripts.Services
{
    public class UIFactory : IUIFactory
    {
        private const string CountdownTimerPath = "CountdownTimer";
        private const string UIRootPath = "UI/Root";
        private const string UITimeControllerPath = "UI/TimerController";
        private const string UITimerView = "UI/TimerView";
        private const string UINewTimerButtonPath = "UI/NewTimerButton";
        private const string UITimersContainer = "UI/TimersContainer";

        private readonly IProgressService _progressService;
        private readonly ISaveLoadService _saveLoadService;
        private readonly TweenController _tweenController;
        private readonly Updater _updater;
        private readonly LoadingCurtain _curtain;

        private UIRoot _uiRoot;
        private TimerContainer _timerContainer;

        public UIFactory(IProgressService progressService, ISaveLoadService saveLoadService,
            TweenController tweenController, Updater updater, LoadingCurtain curtain)
        {
            _progressService = progressService;
            _saveLoadService = saveLoadService;
            _tweenController = tweenController;
            _updater = updater;
            _curtain = curtain;
        }

        public void CreateUIRoot()
        {
            _uiRoot = Instantiate(UIRootPath).GetComponent<UIRoot>();
            _uiRoot.Init();
        }

        public void CreateTimerContainer()
        {
            float containerHeight = 0.5f * (_uiRoot.GetHeight() / 2);
            Vector2 spawnPosition = new Vector2(0, _uiRoot.StartPositionToFirstTimer.y);

            _timerContainer = Instantiate(UITimersContainer, _uiRoot.UIRootTransform)
                .GetComponent<TimerContainer>();
            _timerContainer.Construct(spawnPosition, containerHeight);
        }

        public void CreateCountDownTimer()
        {
            CountdownTimer countdownTimer = Instantiate(CountdownTimerPath).GetComponent<CountdownTimer>();
            countdownTimer.Construct(_tweenController, _progressService, _saveLoadService, this, _curtain,
                _uiRoot, _timerContainer);

            countdownTimer.StartApp();
        }

        public TimerController CreateTimer(int ordinalNumber, Vector2 spawnPosition)
        {
            TimerController timer = Instantiate(UITimeControllerPath, _uiRoot.UIRootTransform)
                .GetComponent<TimerController>();

            if (ordinalNumber == 0)
            {
                spawnPosition.x -= 0.55f * timer.GetComponent<RectTransform>().rect.width;
            }

            timer.GetComponent<RectTransform>().anchoredPosition = spawnPosition;

            timer.Construct(this, _tweenController, _uiRoot.UIRootTransform);
            timer.Init(_progressService.TimerSave.GetTimerData(ordinalNumber), _updater);

            return timer;
        }

        public TimerView CreateTimerView(TimerController timerController, Vector2 spawnPosition)
        {
            TimerView timerView = Instantiate(UITimerView, _uiRoot.UIRootTransform).GetComponent<TimerView>();

            RectTransform timerViewTransform = timerView.GetComponent<RectTransform>();
            spawnPosition.y += 0.55f * timerViewTransform.rect.height;
            timerViewTransform.anchoredPosition = spawnPosition;

            return timerView;
        }

        public RectTransform CreateNewTimerButton(Vector2 spawnPosition)
        {
            RectTransform newTimerButton = Instantiate(UINewTimerButtonPath, _uiRoot.UIRootTransform)
                .GetComponent<RectTransform>();

            newTimerButton.anchoredPosition = spawnPosition;

            return newTimerButton;
        }

        private static GameObject Instantiate(string objectPath) =>
            Object.Instantiate(GetObjectPrefab(objectPath));

        private static GameObject Instantiate(string objectPath, RectTransform parent) =>
            Object.Instantiate(GetObjectPrefab(objectPath), parent);

        private static GameObject GetObjectPrefab(string objectPath) =>
            Resources.Load<GameObject>(objectPath);
    }
}