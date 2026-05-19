namespace Server.Application.DTOs
{
    public class EAFDto
    {
        public bool Load_scrap { get; set; } 
        public double Current_setpoint { get; set; } 
        public bool Tap { get; set; }  
        public double Tap_angle { get; set; } 
        public bool Reset { get; set; }  

        public double Mass_tons { get; set; }  
        public double Temperature_C { get; set; } 
        public double Energy_consumed { get; set; } 

        public bool Furnace_overfill { get; set; }
        public bool Furnace_empty { get; set; }
        public bool Furnace_overtemperature { get; set; }
        public bool Tapping_error { get; set; }
    }
}
