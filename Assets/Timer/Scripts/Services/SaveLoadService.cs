using Timer.Scripts.Data;
using UnityEngine;

namespace Timer.Scripts.Services
{
    public class SaveLoadService: ISaveLoadService
    {
        private const string ProgressKey = "Progress";
        private readonly IProgressService _progressService;

        public SaveLoadService(IProgressService progressService) => 
            _progressService = progressService;

        public void Save() => 
            PlayerPrefs.SetString(ProgressKey, _progressService.TimerSave.ToJson());

        public TimerSave Load() => 
            PlayerPrefs.GetString(ProgressKey)?.ToDeserialized<TimerSave>();
    }
}