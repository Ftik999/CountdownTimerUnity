using Timer.Scripts.UI;
using UnityEngine;

namespace Timer.Scripts.Services
{
    public interface IUIFactory
    {
        void CreateUIRoot();
        void CreateCountDownTimer();
        void CreateTimerContainer();
        TimerController CreateTimer(int ordinalNumber, Vector2 spawnPosition);
        TimerView CreateTimerView(TimerController timerController, Vector2 spawnPosition);
        RectTransform CreateNewTimerButton(Vector2 spawnPosition);
    }
}