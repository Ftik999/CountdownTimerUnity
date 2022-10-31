using UnityEngine;
using Object = UnityEngine.Object;

namespace Timer.Scripts
{
    public class UIFactory
    {
        private const string UIRootPath = "UI/Root";
        private const string UITimeControllerPath = "UI/TimerController";
        private const string UITimerView = "UI/TimerView";
        private readonly TweenController _tweenController;

        private RectTransform _uiRoot;

        public UIFactory(TweenController tweenController) =>
            _tweenController = tweenController;

        public RectTransform CreateUIRoot()
        {
            GameObject rootPrefab = Resources.Load<GameObject>(UIRootPath);
            _uiRoot = Object.Instantiate(rootPrefab).GetComponent<RectTransform>();

            return _uiRoot;
        }

        public TimerController CreateTimer(int ordinalNumber, Vector2 startPositionToFirstTimer)
        {
            TimerController timerPrefab = Resources.Load<TimerController>(UITimeControllerPath);
            TimerController timer = Object.Instantiate(timerPrefab, _uiRoot);

            RectTransform timerTransform = timer.GetComponent<RectTransform>();
            startPositionToFirstTimer.x -= 0.55f * timerTransform.rect.width;
            startPositionToFirstTimer.y -= (1.3f * timerTransform.rect.height) * (ordinalNumber - 1);
            timerTransform.anchoredPosition = startPositionToFirstTimer;

            timer.Construct(this, _tweenController, _uiRoot, ordinalNumber);

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
    }
}