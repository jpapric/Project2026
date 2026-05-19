using System.Security.Principal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Server.Application.Interfaces;
using Server.Domain;
using static Server.Domain.Plc;

namespace Server.Infrastructure.Repository
{
    public class ServerRepository : IServerRepository
    {

        private readonly string _connectionString;

        public ServerRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
        }


        public Plc GetPlc()
        {
            try
            {
                Plc result = new Plc() { };

                string query = "SELECT _ip, rack, slot, cpu_type from PLC_CONFIGURATION";

                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string ip = reader.GetString(reader.GetOrdinal("_ip"));
                    int rack = reader.GetInt32(reader.GetOrdinal("rack"));
                    int slot = reader.GetInt32(reader.GetOrdinal("slot"));
                    string cpu_type = reader.GetString(reader.GetOrdinal("cpu_type"));

                    result = new Plc(cpu_type, ip, rack, slot);

                }

                connection.Close();
                return result;

            }
            catch {
                throw new Exception("No PLC configuration found in database.");
            }
           

        }

        public void UpdatePlc(Plc plc)
        {
            string query = "UPDATE plc_configuration " +
                           "SET _ip = @ip, rack = @rack, slot = @slot, cpu_type = @cpu ";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@ip", plc.Ip);
            command.Parameters.AddWithValue("@rack", plc.Rack);
            command.Parameters.AddWithValue("@slot", plc.Slot);
            command.Parameters.AddWithValue("@cpu", plc.Cpu);



            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }

        public void SetCurrent(float current)
        {
            
            string query = "UPDATE L2_TO_PLC " +
                           "SET CURRENT_SETPOINT = @current_setpoint";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@current_setpoint", current);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
            
        }

        public void SetAngle(float angle)
        {

            string query = "UPDATE L2_TO_PLC " +
                           "SET TAP_ANGLE = @angle";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@angle", angle);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }

        public EAF GetEAF()
        {
            try
            {
                EAF result = new EAF();
                string query = @"SELECT Scrap_loading, Tapping_active, Actual_tilting, Material_weight, Actual_current, 
                        Energy_consumed, Actual_temperature, Furnace_overfill, Tapping_error, 
                        Furnace_empty, Furnace_overtemperature 
                        FROM PLC_TO_L2";

                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    bool scrap_loading = reader.GetBoolean(reader.GetOrdinal("Scrap_loading"));
                    bool tapping_active = reader.GetBoolean(reader.GetOrdinal("Tapping_active"));
                    float actual_tilting = reader.GetFloat(reader.GetOrdinal("Actual_tilting"));
                    float material_weight = reader.GetFloat(reader.GetOrdinal("Material_weight"));
                    float actual_current = reader.GetFloat(reader.GetOrdinal("Actual_current"));
                    float energy_consumed = reader.GetFloat(reader.GetOrdinal("Energy_consumed"));
                    float actual_temperature = reader.GetFloat(reader.GetOrdinal("Actual_temperature"));
                    bool furnace_overfill = reader.GetBoolean(reader.GetOrdinal("Furnace_overfill"));
                    bool tapping_error = reader.GetBoolean(reader.GetOrdinal("Tapping_error"));
                    bool furnace_empty = reader.GetBoolean(reader.GetOrdinal("Furnace_empty"));
                    bool furnace_overtemperature = reader.GetBoolean(reader.GetOrdinal("Furnace_overtemperature"));

                    result = new EAF(scrap_loading, tapping_active, actual_tilting, material_weight, actual_current,
                                     energy_consumed, actual_temperature, furnace_overfill, tapping_error,
                                     furnace_empty, furnace_overtemperature);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri čitanju EAF podataka: {ex.Message}");
            }
        }
        public void PostEAF(EAF eaf)
        {
            try
            {
                string query = @"INSERT INTO PLC_TO_L2 
                        (Scrap_loading, Tapping_active, Actual_tilting, Material_weight, Actual_current,
                        Energy_consumed, Actual_temperature, Furnace_overfill, Tapping_error,
                        Furnace_empty, Furnace_overtemperature)
                        VALUES
                        (@Scrap_loading, @Tapping_active, @Actual_tilting, @Material_weight, @Actual_current,
                        @Energy_consumed, @Actual_temperature, @Furnace_overfill, @Tapping_error,
                        @Furnace_empty, @Furnace_overtemperature)";

                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Scrap_loading", eaf.Scrap_loading);
                command.Parameters.AddWithValue("@Tapping_active", eaf.Tapping_active);
                command.Parameters.AddWithValue("@Actual_tilting", eaf.Actual_tilting);
                command.Parameters.AddWithValue("@Material_weight", eaf.Material_weight);
                command.Parameters.AddWithValue("@Actual_current", eaf.Actual_current);
                command.Parameters.AddWithValue("@Energy_consumed", eaf.Energy_consumed);
                command.Parameters.AddWithValue("@Actual_temperature", eaf.Actual_temperature);
                command.Parameters.AddWithValue("@Furnace_overfill", eaf.Furnace_overfill);
                command.Parameters.AddWithValue("@Tapping_error", eaf.Tapping_error);
                command.Parameters.AddWithValue("@Furnace_empty", eaf.Furnace_empty);
                command.Parameters.AddWithValue("@Furnace_overtemperature", eaf.Furnace_overtemperature);

                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri upisivanju EAF podataka: {ex.Message}");
            }
        }

        public float GetEnergyConsumed()
        {
            float energy_consumed = 0f;

            try
            {
                string query = "SELECT ENERGY_CONSUMED FROM PLC_TO_L2";
                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    energy_consumed = reader.GetFloat(reader.GetOrdinal("ENERGY_CONSUMED"));
                }
                return energy_consumed;
            }
            catch
            {
                throw new Exception("No PLC configuration found in database.");
            }
        }

        /*
        public void UpdateShearSpeed(Shear shear) {

        }
        */





        /*
        public void UpdateShearCutLength(float cutLength)
        {

        }
        */



        //public LastEvent(){};
        //public UpdateShears(){};
        //public UpdateMaterial(){};
        //

    }
}





