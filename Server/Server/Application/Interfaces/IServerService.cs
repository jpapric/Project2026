using Server.Application.DTOs;

namespace Server.Application.Interfaces
{
    public interface IServerService
    {
        PlcDto GetPlc();
    }
}