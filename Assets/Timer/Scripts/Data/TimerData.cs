using System;

namespace Timer.Scripts.Data
{
    [Serializable]
    public class TimerData
    {
        public int Id;
        public long StartTimeToUnix;
        public double TimerTime;
        public bool IsComplete;

        public TimerData(int id) => 
            Id = id;
    }
}