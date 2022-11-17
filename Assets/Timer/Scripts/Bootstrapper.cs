using Timer.Scripts.Data;
using Timer.Scripts.Services;
using Timer.Scripts.UI;
using UnityEngine;

namespace Timer.Scripts
{
    public class Bootstrapper : MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private LoadingCurtain _curtain;

        private TweenController _tweenController;
        private IProgressService _progressService;
        private ISaveLoadService _saveLoadService;
        private IUIFactory _factory;

        private void Awake() =>
            Init();

        private void Init()
        {
            Updater updater = gameObject.AddComponent<Updater>();

            _tweenController = new TweenController(this);
            _progressService = new ProgressService();
            _saveLoadService = new SaveLoadService(_progressService);
            _factory = new UIFactory(_progressService, _saveLoadService, _tweenController, updater, _curtain);

            InitUI();
            LoadOrInitialProgress();
            InitCountdownTimer();
        }

        private void InitUI()
        {
            _factory.CreateUIRoot();
            _factory.CreateTimerContainer();
        }

        private void InitCountdownTimer() =>
            _factory.CreateCountDownTimer();

        private void LoadOrInitialProgress() =>
            _progressService.TimerSave = _saveLoadService.Load() ?? new TimerSave();
    }
}