using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class EAFDto
    {
        public bool Scrap_loading { get; set; }
        public bool Tapping_active { get; set; }
        public bool Electrodes_lowered { get; set; }
        public bool Electrodes_moving { get; set; }
        public float Actual_tilting { get; set; }
        public float Material_weight { get; set; }
        public float Actual_current { get; set; }
        public float Energy_consumed { get; set; }
        public float Actual_temperature { get; set; }

        // Status / Error Flags
        public bool Furnace_overfill { get; set; }
        public bool Tapping_error { get; set; }
        public bool Furnace_empty { get; set; }
        public bool Furnace_overtemperature { get; set; }
    }
}
