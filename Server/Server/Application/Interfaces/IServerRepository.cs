using Server.Domain;

namespace Server.Application.Interfaces
{
    public interface IServerRepository
    {
        Plc GetPlc();

        void UpdatePlc(Plc plc);
    }
}