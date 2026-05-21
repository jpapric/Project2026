using Client.Models;
using Client.Proxies;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Client.ViewModel
{
    public class EAFViewModel
    {
        private readonly EAFProxy _eafProxy = new EAFProxy();

        public async Task PlcConnectionTab()
        {
            PLCDto plc = await _eafProxy.GetPlc();
            PlcConnectionViewModel plcConnectionTab = new PlcConnectionViewModel(plc);
        }

    }
}
