namespace Server.Application.DTOs
{
    public class EAFDto
    {
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
    }
}