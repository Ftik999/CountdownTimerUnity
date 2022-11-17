using System;
using Timer.Scripts.Data;
using Timer.Scripts.Services;
using UniRx;

namespace Timer.Scripts
{
    public class Timer : IUpdateListener
    {
        private const int SecondsOnDay = 86400;
        private const int ValueForChange = 5;
        private const float TimeDelay = 0.7f;
        private const float PressedValueCoefficient = 0.4f;

        private readonly TimerData _timerData;

        private bool _addButtonLongPressIsActive;
        private bool _removeButtonLongPressIsActive;
        private bool _isStarted;
        private bool _isComplete;
        private float _buttonPressedTime;
        private long _startTimeToUnix;
        public bool IsComplete => _isComplete;

        public DoubleReactiveProperty TimerTime = new();
        public ReactiveCommand TimerStopped = new ReactiveCommand();

        public Timer(TimerData timerData, Updater updater)
        {
            _timerData = timerData;
            updater.AddListener(this);

            Init();
        }

        public void Update(float deltaTime)
        {
            if (_addButtonLongPressIsActive || _removeButtonLongPressIsActive)
            {
                ChangeTimerValueWhenButtonIsPressed(deltaTime);
            }

            TimerLogic(deltaTime);
        }

        public void AddTimerValue(bool isPressed)
        {
            _addButtonLongPressIsActive = isPressed;

            if (_addButtonLongPressIsActive)
            {
                _removeButtonLongPressIsActive = false;

                _buttonPressedTime = 0f;
                IncreaseTime(ValueForChange);
            }
        }

        public void RemoveTimeValue(bool isPressed)
        {
            _removeButtonLongPressIsActive = isPressed;

            if (_removeButtonLongPressIsActive)
            {
                _addButtonLongPressIsActive = false;

                _buttonPressedTime = 0f;
                DecreaseTime(ValueForChange);
            }
        }

        public void StartTimer()
        {
            if (TimerTime.Value > 0)
            {
                _isStarted = true;
                _startTimeToUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
            else
            {
                TimerTime.Value = 0;
            }

            _isComplete = false;

            WriteSaves();
        }

        public void TimerReviewed()
        {
            if (_isComplete)
            {
                _isComplete = false;
            }

            WriteSaves();
        }

        private void Init()
        {
            TimerTime.Value = _timerData.TimerTime;
            _startTimeToUnix = _timerData.StartTimeToUnix;
            _isComplete = _timerData.IsComplete;

            TimerSetup();
        }

        private void TimerSetup()
        {
            if (TimerTime.Value > 0)
            {
                var currentUnixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (_startTimeToUnix + TimerTime.Value < currentUnixTime)
                {
                    TimerTime.Value = 0;
                    _isComplete = true;
                }
                else
                {
                    TimerTime.Value = _startTimeToUnix + TimerTime.Value - currentUnixTime;
                    _isStarted = true;
                }
            }
        }

        private void TimerLogic(float deltaTime)
        {
            if (!_isStarted)
            {
                return;
            }

            if (TimerTime.Value > 0)
            {
                TimerTime.Value -= deltaTime;
            }
            else
            {
                Stop();
            }
        }

        private void ChangeTimerValueWhenButtonIsPressed(float deltaTime)
        {
            if (_buttonPressedTime > TimeDelay)
            {
                if (_addButtonLongPressIsActive)
                {
                    IncreaseTime(PressedValueCoefficient * _buttonPressedTime);
                }

                if (_removeButtonLongPressIsActive)
                {
                    DecreaseTime(PressedValueCoefficient * _buttonPressedTime);
                }
            }

            _buttonPressedTime += deltaTime;
        }

        private void IncreaseTime(float value)
        {
            double newTime = TimerTime.Value + value;

            if (newTime > SecondsOnDay)
            {
                ChangeTimerValue(_isStarted ? SecondsOnDay : newTime - SecondsOnDay);
            }
            else
            {
                ChangeTimerValue(newTime);
            }
        }

        private void DecreaseTime(float value)
        {
            double newTime = TimerTime.Value - value;

            if (newTime < 0)
            {
                ChangeTimerValue(SecondsOnDay - newTime * -1);
            }
            else
            {
                ChangeTimerValue(newTime);
            }
        }

        private void ChangeTimerValue(double newTime) =>
            TimerTime.Value = newTime;

        private void Stop()
        {
            _isStarted = false;
            TimerTime.Value = 0;
            _startTimeToUnix = 0;
            _isComplete = true;


            WriteSaves();
            TimerStopped?.Execute();
        }

        private void WriteSaves()
        {
            _timerData.IsComplete = _isComplete;
            _timerData.TimerTime = TimerTime.Value;
            _timerData.StartTimeToUnix = _startTimeToUnix;
        }
    }
}