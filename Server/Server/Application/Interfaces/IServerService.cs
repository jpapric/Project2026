using S7.Net;
using Server.Application.DTOs;

namespace Server.Application.Interfaces
{
    public interface IServerService
    {
        PlcDto GetPlc();

        void UpdatePlc(PlcDto plcdto);

    }
}






