namespace Server.Domain
{
    public class EAF
    {
        public EAF() { }

        public EAF(
            bool scrap_loading,
            bool tapping_active,
            float actual_tilting,
            float material_weight,
            float actual_current,
            float energy_consumed,
            float actual_temperature,
            bool furnace_overfill,
            bool tapping_error,
            bool furnace_empty,
            bool furnace_overtemperature)
        {
            Scrap_loading = scrap_loading;
            Tapping_active = tapping_active;
            Actual_tilting = actual_tilting;
            Material_weight = material_weight;
            Actual_current = actual_current;
            Energy_consumed = energy_consumed;
            Actual_temperature = actual_temperature;
            Furnace_overfill = furnace_overfill;
            Tapping_error = tapping_error;
            Furnace_empty = furnace_empty;
            Furnace_overtemperature = furnace_overtemperature;
        }

<<<<<<< HEAD
        public bool Scrap_loading { get; } 
        public bool Tapping_active { get; }
        public float Actual_tilting { get; }
        public float Material_weight { get; }
        public float Actual_current { get; }
        public float Energy_consumed { get; }
        public float Actual_temperature { get; }

        
        public bool Furnace_overfill { get; }
        public bool Tapping_error { get; }
        public bool Furnace_empty { get; }
        public bool Furnace_overtemperature { get; }
=======
        public bool Scrap_loading { get; set; }
        public bool Tapping_active { get; set; }
        public float Actual_tilting { get; set; }
        public float Material_weight { get; set; }
        public float Actual_current { get; set; }
        public float Energy_consumed { get; set; }
        public float Actual_temperature { get; set; }


        public bool Furnace_overfill { get; set; }
        public bool Tapping_error { get; set; }
        public bool Furnace_empty { get; set; }
        public bool Furnace_overtemperature { get; set; }
>>>>>>> plc_comm
    }
}