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



        // public void  GetWarnings() { };
        //public GetHistory(){};
        //public GetLastEvent(){};
        //public UpdateShears(){};
        //public UpdateMaterial(){};
        //

    }
}
