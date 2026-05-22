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

        /*public void SetCurrent(EAFDto eafDto)
        {
            EAF eaf = new EAF(
                eafDto.Scrap_loading,
                eafDto.Tapping_active,
                eafDto.Actual_tilting,
                eafDto.Material_weight,
                eafDto.Actual_current,
                eafDto.Energy_consumed,
                eafDto.Actual_temperature,
                eafDto.Furnace_overfill,
                eafDto.Tapping_error,
                eafDto.Furnace_empty,
                eafDto.Furnace_overtemperature
            );
            _repository.SetCurrent(eaf);
        }*/

        public void SetCurrent(float current)
        {
            _repository.SetCurrent(current);
        }
        public void SetAngle(float angle)
        {
            _repository.SetAngle(angle);
        }
        public float GetEnergyConsumed()
        {
            float energy_consumed = _repository.GetEnergyConsumed();

            return energy_consumed;
        }

        public EAFDto GetEAF()
        {
            EAF eaf = _repository.GetEAF();

            EAFDto eafDto = ApplicationFactory.GetEAFDtoFromDomain(eaf);

            return eafDto;
        }
        public void PostEAF(EAFDto eafDto)
        {
            EAF eaf = new EAF(eafDto.Scrap_loading, eafDto.Tapping_active, eafDto.Electrodes_lowered, eafDto.Electrodes_moving, eafDto.Actual_tilting, eafDto.Material_weight, eafDto.Actual_current,
                                     eafDto.Energy_consumed, eafDto.Actual_temperature, eafDto.Furnace_overfill, eafDto.Tapping_error,
                                     eafDto.Furnace_empty, eafDto.Furnace_overtemperature);
            _repository.PostEAF(eaf);
        }
        public async Task LoadScrap()
        {
            await _repository.LoadScrap();
        }
        public async Task Tap()
        {
            await _repository.Tap();
        }
        public async Task Reset()
        {
            await _repository.Reset();
        }
        public async Task LiftElectrodes()
        {
            await _repository.LiftElectrodes();
        }
        public async Task DropElectrodes()
        {
            await _repository.DropElectrodes();
        }
        public List<EventDto> GetEvents()
        {
            
            List<Event> events = _repository.GetEvents();
            return events.Select(e => ApplicationFactory.GetEventDtoFromDomain(e)).ToList();
 
        }
        
    }
}

