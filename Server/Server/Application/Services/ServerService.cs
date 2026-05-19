using Server.Application.DTOs;
using Server.Application.Interfaces;
using Server.Domain;


namespace Server.Application.Services
{
    public class ServerService : IServerService
    {
        private IServerRepository _repository;
        public ServerService(IServerRepository repository)
        {
            _repository = repository;
        }


        public PlcDto GetPlc()
        {
            Plc plc = _repository.GetPlc();

            PlcDto plcDto = ApplicationFactory.GetPlcDtofromDomain(plc);

            return plcDto;
        }

        public void UpdatePlc(PlcDto plcdto) 
        {
            Plc plc = new Plc(plcdto.Cpu, plcdto.Ip, plcdto.Rack, plcdto.Slot);
            _repository.UpdatePlc(plc);
        }

    }
}

