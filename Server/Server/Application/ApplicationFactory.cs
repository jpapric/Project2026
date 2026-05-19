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
                result.Add(GetEAFDtofromDomain(e));
            }

            return result;
        }

        public static EAFDto GetEAFDtofromDomain(EAF eaf)
        {
            return new EAFDto
            {
                Load_scrap = eaf.Load_scrap,
                Current_setpoint = eaf.Current_setpoint,
                Tap = eaf.Tap,
                Tap_angle = eaf.Tap_angle,
                Reset = eaf.Reset,
                Mass_tons = eaf.Mass_tons,
                Temperature_C = eaf.Temperature_C,
                Energy_consumed = eaf.Energy_consumed,
                Furnace_overfill = eaf.Furnace_overfill,
                Furnace_empty = eaf.Furnace_empty,
                Furnace_overtemperature = eaf.Furnace_overtemperature,
                Tapping_error = eaf.Tapping_error,

            };
        }
    }
}
