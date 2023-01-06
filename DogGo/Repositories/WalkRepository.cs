using System.Collections.Generic;

using DogGo.Interfaces;
using DogGo.Models;

using Microsoft.AspNetCore.Connections;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;
        public WalkRepository(IConfiguration config)
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

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT w.Id, w.Date, w.Duration, w.WalkerId,
                               d.Id DogId, d.[Name] DogName,
                               o.Id OwnerId, o.[Name] OwnerName
                        FROM Walks w
                        LEFT JOIN Dog d ON d.Id = w.DogId
                        LEFT JOIN Owner o ON d.OwnerId = o.Id
                        WHERE w.Id = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", walkerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Walk> walks = new List<Walk>();

                        while (reader.Read())
                        {
                            Walk walk = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                                DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Dog = new Dog() // Although could just use new(), in this case I opted to specify the type for readability
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                    Name = reader.GetString(reader.GetOrdinal("DogName")),
                                    OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                    Owner = new Owner
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                        Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                                    }
                                }
                            };
                            walks.Add(walk);
                        }

                        return walks;
                    }
                }
            }
        }

        public void CreateWalk(Walk walk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Walks (Date, Duration, WalkerId, DogId)
                        VALUES (@date, @duration, @walkerId, @dogId
                    ";
                    cmd.Parameters.AddWithValue("@date", walk.Date);
                    cmd.Parameters.AddWithValue("@duration", walk.Duration);
                    cmd.Parameters.AddWithValue("@walkerId", walk.WalkerId);
                    cmd.Parameters.AddWithValue("dogId", walk.DogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
