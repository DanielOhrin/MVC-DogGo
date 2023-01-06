using System.Collections.Generic;

using DogGo.Interfaces;
using DogGo.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        public readonly IConfiguration _config;
        public DogRepository(IConfiguration config)
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

        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                    ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Dog> dogs = new List<Dog>();

                        while (reader.Read())
                        {
                            Dog newDog = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ? reader.GetString(reader.GetOrdinal("Notes")) : null,
                                ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? reader.GetString(reader.GetOrdinal("ImageUrl")) : null
                            };

                            dogs.Add(newDog);
                        }

                        return dogs;
                    }
                }
            }
        }
        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Dog ([Name], OwnerId, Breed, Notes, ImageUrl)
                        VALUES (@name, @ownerId, @breed, @notes, @imageUrl)
                    ";
                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM Dog
                        WHERE Id = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", dog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                        UPDATE Dog
                        SET 
                            [Name] = @name,
                            OwnerId = @ownerId
                            {(dog.Breed != null ? ", Breed = @breed" : "")}
                            {(dog.Notes != null ? ", Notes = @notes" : "")}
                            {(dog.ImageUrl != null ? ", ImageUrl = @imageUrl" : "")}
                        WHERE Id = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", dog.Id);
                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);

                    if (dog.Breed != default(string))
                    {
                        cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    }
                    if (dog.Notes != default(string))
                    {
                       cmd.Parameters.AddWithValue("@notes", dog.Notes);
                    }
                    if (dog.ImageUrl != default(string))
                    {
                    cmd.Parameters.AddWithValue("@imageUrl", dog.ImageUrl);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }
        
        public Dog GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, [Name], OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                        WHERE Id = @id
                    ";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Dog dog = null;

                        if (reader.Read())
                        {
                            dog = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ? reader.GetString(reader.GetOrdinal("Notes")) : string.Empty,
                                ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? reader.GetString(reader.GetOrdinal("ImageUrl")) : string.Empty
                            };
                        }

                        return dog;
                    }
                }
            }
        }

        public List<Dog> GetDogsByOwnerId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, Breed, Notes, ImageUrl, OwnerId
                        FROM Dog
                        WHERE OwnerId = @ownerId
                    ";

                    cmd.Parameters.AddWithValue("@ownerId", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Dog> dogs = new();

                        while (reader.Read())
                        {
                            Dog dog = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Breed = reader.GetString(reader.GetOrdinal("Breed")),
                                OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                ImageUrl = reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? null : reader.GetString(reader.GetOrdinal("ImageUrl"))
                            };

                            dogs.Add(dog);
                        }

                        return dogs;
                    }
                }
            }
        }
    }
}
