using Timer.Scripts.Data;

namespace Timer.Scripts.Services
{
    public interface ISaveLoadService
    {
        void Save();
        TimerSave Load();
    }
}