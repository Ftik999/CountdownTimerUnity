using System.Collections.Generic;
using UnityEngine;

namespace Timer.Scripts.Services
{
    public class Updater : MonoBehaviour
    {
        private readonly List<IUpdateListener> _updateListeners = new List<IUpdateListener>();

        public void AddListener(IUpdateListener listener)
        {
            _updateListeners.Add(listener);
        }

        public void RemoveListener(IUpdateListener listener)
        {
            if (_updateListeners.Contains(listener))
            {
                _updateListeners.Remove(listener);
            }
        }

        private void Update()
        {
            for (var i = 0; i < _updateListeners.Count; i++)
            {
                _updateListeners[i].Update(Time.deltaTime);
            }
        }

    }
}