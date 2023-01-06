using System.Collections.Generic;

using DogGo.Interfaces;
using DogGo.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class NeighborhoodRepository : INeighborhoodRepository
    {
        private readonly IConfiguration _config;
        public NeighborhoodRepository(IConfiguration config)
        {
            _config = config;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Neighborhood> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Neighborhood";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Neighborhood> neighborhoods = new();

                        while (reader.Read())
                        {
                            Neighborhood neighborhood = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                            neighborhoods.Add(neighborhood);
                        }

                        return neighborhoods;
                    }
                }
            }
        }
    }
}
