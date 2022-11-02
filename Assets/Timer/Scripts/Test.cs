using System;
using System.Collections.Generic;
using System.Linq;
using Timer.Scripts.Data;
using UnityEngine;

namespace Timer.Scripts
{
    public class Test : MonoBehaviour
    {
        //public TimerData TimerData;
        private List<TimerData> _timerData = new List<TimerData>();

        private void Start()
        {
            // _timerData.Add(new TimerData(1));
            // if (_timerData.Count > 0)
            // {
            //     var TimerData = _timerData.FirstOrDefault(x => x.Id == 3);
            //
            //     if (TimerData == null)
            //     {
            //         Debug.Log("TimerData is NULL");
            //     }
            // }
            
            Debug.Log(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        }
    }
}