using Server.Domain;

namespace Server.Application.Interfaces
{
    public interface IServerRepository
    {
        Plc GetPlc();
        Task LoadScrap();
        void UpdatePlc(Plc plc);
        void SetCurrent(float current);
        void SetAngle(float angle);
        float GetEnergyConsumed();
        EAF GetEAF();
        void PostEAF(EAF eaf);
        Task Tap();
        Task Reset();
    }
}