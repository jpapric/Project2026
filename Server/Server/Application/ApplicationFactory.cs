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
    }
}
