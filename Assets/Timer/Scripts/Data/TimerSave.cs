using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Timer.Scripts.Data
{
    [Serializable]
    public class TimerSave
    {
        public List<TimerData> TimerData;

        public TimerSave()
        {
            TimerData = new List<TimerData>();

            for (int i = 0; i < 3; i++)
            {
                TimerData.Add(new TimerData(i));
            }
        }
        
        public TimerData GetTimerData(int id)
        {
            if (TimerData.Count > 0)
            {
                return TimerData.First(x => x.Id == id);
            }
            
            Debug.LogError("Something wrong :: TimerData Count 0");
            return null;
        }
    }
}