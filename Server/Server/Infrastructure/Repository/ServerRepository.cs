using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Server.Application.Interfaces;
using Server.Domain;
using Server.Infrastructure.BackgroundServices;
using System.Security.Principal;
using System.Security.Principal;
using static Server.Domain.Plc;
using static Server.Infrastructure.BackgroundServices.PlcConnection;

namespace Server.Infrastructure.Repository
{
    public class ServerRepository : IServerRepository
    {

        private readonly string _connectionString;
        private readonly PlcConnection _plcConnection;

        private bool _isLoadingScrap = false;





        public ServerRepository(IConfiguration configuration ,PlcConnection plcConnection)
        {
            _connectionString = configuration.GetConnectionString("DbConnectionString");
            _plcConnection = plcConnection;
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
            _plcConnection.WriteReal("current_setpoint", current);

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
            _plcConnection.WriteReal("tap_angle", angle);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();

        }
        public async Task LoadScrap()
        {
           
            try
            {
                string query = "UPDATE L2_TO_PLC SET Load_scrap = @Load_scrap";

                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                command.Parameters.AddWithValue("@Load_scrap", true);


                command.ExecuteNonQuery();
                _plcConnection.WriteBool("load_scrap", false);
                await Task.Delay(100);
                _plcConnection.WriteBool("load_scrap", true);

               

                

                
                command.Parameters["@Load_scrap"].Value = false;
                
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri Load_scrap upisu: {ex.Message}");
            }
        }
        public async Task Tap()
        {
            string query = "UPDATE L2_TO_PLC SET TAP = @Tap";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Tap", true);
            connection.Open();
            command.ExecuteNonQuery();

            
            _plcConnection.WriteBool("tap",true);
            await Task.Delay(500);

            

            command.Parameters["@Tap"].Value = false;
            command.ExecuteNonQuery();

            connection.Close();
        }
        public async Task LiftElectrodes()
        {
            string query = "UPDATE L2_TO_PLC SET Electrodes = @Electrodes";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Electrodes", false);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public async Task DropElectrodes()
        {
            string query = "UPDATE L2_TO_PLC SET Electrodes = @Electrodes";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Electrodes", true);
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
        public async Task Reset()
        {
            string query = "UPDATE L2_TO_PLC                          " +
                           "SET RESET = @Reset                        " ;

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@Reset", true);

            connection.Open();
            command.ExecuteNonQuery();
            _plcConnection.WriteBool("reset", true);
            await Task.Delay(100);
            _plcConnection.WriteBool("reset", false);
            command.Parameters["@Reset"].Value = false;
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

        public void PostEvent(string name, string type, DateTime time)
        {
            string query = @"INSERT INTO EVENTS (EVENT_NAME, EVENT_TYPE, EVENT_TIME) 
                         VALUES (@name, @type, @time)";

            using SqlConnection connection = new SqlConnection(_connectionString);
            using SqlCommand command = new SqlCommand(query, connection);

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@type", type);
            command.Parameters.AddWithValue("@time", time);
            connection.Open();
            command.ExecuteNonQuery();

            connection.Close();
        }
        public List<Event> GetEvents()
        {
            try
            {
                List<Event> events = new List<Event>();
                string query = @"SELECT EVENT_NAME, EVENT_TYPE, EVENT_TIME
                        FROM EVENTS 
                        ORDER BY EVENT_TIME DESC";

                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    events.Add(new Event(
                        reader.GetString(reader.GetOrdinal("EVENT_NAME")),
                        reader.GetString(reader.GetOrdinal("EVENT_TYPE")),
                        reader.GetDateTime(reader.GetOrdinal("EVENT_TIME"))
                    ));
                }

                return events;
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri čitanju evenata: {ex.Message}");
            }
        }
        public void Event_detection(EAF current)
        {
            try
            {
                string query = @"SELECT TOP 1 Scrap_loading, Tapping_active, Actual_tilting, 
                                Material_weight, Actual_current, Energy_consumed, Actual_temperature, 
                                Furnace_overfill, Tapping_error, Furnace_empty, Furnace_overtemperature
                                FROM PLC_TO_L2
                                ORDER BY Id DESC";

                using SqlConnection connection = new SqlConnection(_connectionString);
                using SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();

                EAF previous = null;

                if (reader.Read())
                    previous = ReadEAFFromReader(reader);

                if (previous == null) return;

                if (!previous.Scrap_loading && current.Scrap_loading)
                    PostEvent("Loading scrap", "Feedback", DateTime.Now);
                else if (previous.Scrap_loading && !current.Scrap_loading)
                    PostEvent("Loading scrap finished", "Feedback", DateTime.Now);

                if (!previous.Tapping_active && current.Tapping_active)
                    PostEvent("Tapping started", "Feedback", DateTime.Now);
                else if (previous.Tapping_active && !current.Tapping_active)
                    PostEvent("Tapping finished", "Feedback", DateTime.Now);

                if (!previous.Furnace_overfill && current.Furnace_overfill)
                    PostEvent("Furnace overfilled", "Warning", DateTime.Now);
                else if (previous.Furnace_overfill && !current.Furnace_overfill)
                    PostEvent("Furnace overfill resolved", "Feedback", DateTime.Now);

                if (!previous.Tapping_error && current.Tapping_error)
                    PostEvent("Tapping error", "Warning", DateTime.Now);
                else if (previous.Tapping_error && !current.Tapping_error)
                    PostEvent("Tapping error resolved", "Feedback", DateTime.Now);

                if (!previous.Furnace_empty && current.Furnace_empty)
                    PostEvent("Furnace empty", "Warning", DateTime.Now);
                else if (previous.Furnace_empty && !current.Furnace_empty)
                    PostEvent("Furnace filled", "Feedback", DateTime.Now);

                if (!previous.Furnace_overtemperature && current.Furnace_overtemperature)
                    PostEvent("Furnace overheated", "Warning", DateTime.Now);
                else if (previous.Furnace_overtemperature && !current.Furnace_overtemperature)
                    PostEvent("Furnace overtemperature resolved", "Feedback", DateTime.Now);
            }
            catch (Exception ex)
            {
                throw new Exception($"Greška pri detekciji eventa: {ex.Message}");
            }
        }
        private EAF ReadEAFFromReader(SqlDataReader reader)
        {
            return new EAF(
                reader.GetBoolean(reader.GetOrdinal("Scrap_loading")),
                reader.GetBoolean(reader.GetOrdinal("Tapping_active")),
                Convert.ToSingle(reader["Actual_tilting"]),
                Convert.ToSingle(reader["Material_weight"]),
                Convert.ToSingle(reader["Actual_current"]),
                Convert.ToSingle(reader["Energy_consumed"]),
                Convert.ToSingle(reader["Actual_temperature"]),
                reader.GetBoolean(reader.GetOrdinal("Furnace_overfill")),
                reader.GetBoolean(reader.GetOrdinal("Tapping_error")),
                reader.GetBoolean(reader.GetOrdinal("Furnace_empty")),
                reader.GetBoolean(reader.GetOrdinal("Furnace_overtemperature"))
          
            );
        }

        //public LastEvent(){};
        //public UpdateShears(){};
        //public UpdateMaterial(){};
        //


    }
}





