using S7.Net.Protocol;
using Server.Application.DTOs;
using Server.Domain;

namespace Server.Application
{
    public class ApplicationFactory
    {

        public static List<PlcDto> GetPlcsDtosFromDomain(List<Plc> plcs)
        {
            List<PlcDto> result = new List<PlcDto>();

            foreach (Plc p in plcs)
            {
                result.Add(GetPlcDtofromDomain(p));
            }

            return result;
        }

        public static PlcDto GetPlcDtofromDomain(Plc plc)
        {
            return new PlcDto
            {
                Cpu = plc.Cpu,
                Ip = plc.Ip,
                Rack = plc.Rack,
                Slot = plc.Slot
            };
        }

        public static List<EAFDto> GetEAFsDtosFromDomain(List<EAF> eafs)
        {
            List<EAFDto> result = new List<EAFDto>();

            foreach (EAF e in eafs)
            {
                result.Add(GetEAFDtoFromDomain(e));
            }

            return result;
        }

        public static EAFDto GetEAFDtoFromDomain(EAF eaf)
        {
            return new EAFDto
            {
                Scrap_loading = eaf.Scrap_loading,
                Tapping_active = eaf.Tapping_active,
                Actual_tilting = eaf.Actual_tilting,
                Material_weight = eaf.Material_weight,
                Actual_current = eaf.Actual_current,
                Energy_consumed = eaf.Energy_consumed,
                Actual_temperature = eaf.Actual_temperature,
                Furnace_overfill = eaf.Furnace_overfill,
                Tapping_error = eaf.Tapping_error,
                Furnace_empty = eaf.Furnace_empty,
                Furnace_overtemperature = eaf.Furnace_overtemperature,
            };
        }
    }
}