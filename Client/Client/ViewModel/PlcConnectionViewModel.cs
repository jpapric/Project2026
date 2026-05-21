using Client.Helpers;
using Client.Models;
using Client.Proxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using WpfApp1.Helpers;
using static Client.Models.PLCDto;
using  static Client.Proxies.EAFProxy;

namespace Client.ViewModel
{
    public class PlcConnectionViewModel
    {
        public event Action OnSaved;
        public string Ip;
        public int Rack;
        public int Slot;
        public CpuType Cpu;


        private readonly EAFProxy _proxy = new EAFProxy();
        public ICommand ConnectCommand { get; set; }



        /*
        public  PlcConnectionViewModel(PLCDto plc)
        {

            Ip = plc.Ip;
            Rack = plc.Rack;
            Slot = plc.Slot;
            Cpu = plc.Cpu;

            ConnectCommand = new AsyncCommand(Connect);

        }

       */
        public PlcConnectionViewModel() {
            ConnectCommand = new AsyncCommand(Connect);
        }
        private async Task Connect()
        {
            PLCDto result = new PLCDto
            {
                Ip = Ip,
                Rack = Rack,
                Slot = Slot,
                Cpu = Cpu

            };

            await _proxy.UpdatePlc(result); 
        }




   
}
}


