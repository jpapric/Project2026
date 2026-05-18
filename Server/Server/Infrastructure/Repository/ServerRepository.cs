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

                result = new Plc(cpu_type,ip, rack, slot);

            }
            connection.Close();
            return result;

        }
    }
}
