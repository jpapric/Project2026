namespace Server.Domain
{
    public class EAF
    {
        public EAF(bool load_scrap, double current_setpoint, bool tap, double tap_angle, bool reset, double mass_tons, double temperature_C, double energy_consumed, bool furnace_overfill, bool furnace_empty, bool furnace_overtemperature, bool tapping_error)
        {
            Load_scrap = load_scrap;
            Current_setpoint = current_setpoint;
            Tap = tap;
            Tap_angle = tap_angle;
            Reset = reset;
            Mass_tons = mass_tons;
            Temperature_C = temperature_C;
            Energy_consumed = energy_consumed;
            Furnace_overfill = furnace_overfill;
            Furnace_empty = furnace_empty;
            Furnace_overtemperature = furnace_overtemperature;
            Tapping_error = tapping_error;
        }

        // --- INPUTS (naredbe prema PLC-u) ---
        public bool Load_scrap { get; }  // impuls: dodaj 1t otpada
        public double Current_setpoint { get; }  // 0–100 kA
        public bool Tap { get; }  // enable izlijevanja
        public double Tap_angle { get; }  // 0–45°
        public bool Reset { get; }  // reset simulatora

        // --- OUTPUTS (čitanje stanja iz PLC-a) ---
        public double Mass_tons { get; }  // trenutna masa [t]
        public double Temperature_C { get; }  // temperatura [°C]
        public double Energy_consumed { get; }  // MWh potrošeno

        // --- ALARMI ---
        public bool Furnace_overfill { get; }
        public bool Furnace_empty { get; }
        public bool Furnace_overtemperature { get; }
        public bool Tapping_error { get; }

    }
}
