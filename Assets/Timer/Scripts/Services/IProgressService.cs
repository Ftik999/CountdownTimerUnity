using Timer.Scripts.Data;

namespace Timer.Scripts.Services
{
    public interface IProgressService
    {
        TimerSave TimerSave { get; set; }
    }
}