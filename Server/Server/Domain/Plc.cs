using Microsoft.Extensions.Primitives;
using System.ComponentModel;

namespace Server.Domain
{
    public class Plc
    {
        public string Ip { get; }
        public int Rack { get; }
        public int Slot { get; }

        public string Cpu { get; }

        public enum CpuType
        {
            S7200 = 0,
            S7300 = 10,
            S7400 = 20,
            S71200 = 30,
            S71500 = 40
        }
        //constructor call should be :  Plc plc = new Plc(CpuType.S7300, "127.0.0.1", 0, 2); but its  Plc plc = new Plc(S7300, "127.0.0.1", 0, 2);

        public Plc() { }
        public Plc(string cpu, string ip, int rack, int slot)
        {
            Cpu = cpu;
            Ip = ip;
            Rack = rack;
            Slot = slot;
        }
    }
}

