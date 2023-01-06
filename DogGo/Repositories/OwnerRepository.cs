using System.Collections.Generic;

using DogGo.Interfaces;
using DogGo.Models;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DogGo.Repositories
{
    public class OwnerRepository : IOwnerRepository
    {
        private readonly IConfiguration _config;
        public OwnerRepository(IConfiguration config)
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
        public List<Owner> GetAllOwners()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id, o.Email, o.[Name], o.[Address], o.NeighborhoodId, o.Phone, n.Name nName
                        FROM Owner o
                        LEFT JOIN Neighborhood n ON n.Id = o.NeighborhoodId
                    ";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Owner> owners = new();

                        while (reader.Read())
                        {
                            Owner newOwner = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Neighborhood = new Neighborhood { Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")), Name = reader.GetString(reader.GetOrdinal("nName")) }
                            };
                            owners.Add(newOwner);
                        }

                        return owners;
                    }
                }
            }
        }
        public Owner GetByOwnerId(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id, o.Email, o.[Name], o.[Address], o.NeighborhoodId, o.Phone, 
                        d.Id DogId, d.Name DogName, d.Breed, d.Notes, d.ImageUrl,
                        n.Name nName
                        FROM Owner o
                        LEFT JOIN Dog d ON o.Id = d.OwnerId
                        LEFT JOIN Neighborhood n ON n.Id = o.NeighborhoodId
                        WHERE o.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Owner owner = null;

                        if (reader.Read())
                        {
                            owner = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Neighborhood = new Neighborhood { Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")), Name = reader.GetString(reader.GetOrdinal("nName")) },
                                Dogs = reader.IsDBNull(reader.GetOrdinal("DogId"))
                                ? new List<Dog>()
                                : new List<Dog>
                                {
                                    new Dog
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                        Name = reader.GetString(reader.GetOrdinal("DogName")),
                                        Breed = !reader.IsDBNull(reader.GetOrdinal("Breed")) ? reader.GetString(reader.GetOrdinal("Breed")) : string.Empty,
                                        Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ? reader.GetString(reader.GetOrdinal("Notes")) : string.Empty,
                                        ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? reader.GetString(reader.GetOrdinal("ImageUrl")) : string.Empty
                                    }
                                }
                            };
                        }
                        while (reader.Read()) // Used an if statement first to initialize all of the Owner data that will not change
                        {
                            Dog newDog = new()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                                Breed = !reader.IsDBNull(reader.GetOrdinal("Breed")) ? reader.GetString(reader.GetOrdinal("Breed")) : string.Empty,
                                Notes = !reader.IsDBNull(reader.GetOrdinal("Notes")) ? reader.GetString(reader.GetOrdinal("Notes")) : string.Empty,
                                ImageUrl = !reader.IsDBNull(reader.GetOrdinal("ImageUrl")) ? reader.GetString(reader.GetOrdinal("ImageUrl")) : string.Empty
                            };
                            owner.Dogs.Add(newDog);
                        }

                        return owner;
                    }
                }
            }
        }
        public Owner GetOwnerByEmail(string email)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT o.Id, o.[Name], o.Email, o.Address, o.Phone, o.NeighborhoodId, n.Name nName
                        FROM Owner
                        LEFT JOIN Neighborhood n on n.Id = o.NeighborhoodId
                        WHERE Email = @email";

                    cmd.Parameters.AddWithValue("@email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Owner owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                Address = reader.GetString(reader.GetOrdinal("Address")),
                                Phone = reader.GetString(reader.GetOrdinal("Phone")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Neighborhood = new Neighborhood { Id = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")), Name = reader.GetString(reader.GetOrdinal("nName")) }
                            };

                            return owner;
                        }

                        return null;
                    }
                }
            }
        }

        public void AddOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Owner ([Name], Email, Phone, Address, NeighborhoodId)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @email, @phoneNumber, @address, @neighborhoodId);
                ";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@phoneNumber", owner.Phone);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);

                    int id = (int)cmd.ExecuteScalar();

                    owner.Id = id;
                }
            }
        }

        public void UpdateOwner(Owner owner)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Owner
                            SET 
                                [Name] = @name, 
                                Email = @email, 
                                Address = @address, 
                                Phone = @phone, 
                                NeighborhoodId = @neighborhoodId
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", owner.Name);
                    cmd.Parameters.AddWithValue("@email", owner.Email);
                    cmd.Parameters.AddWithValue("@address", owner.Address);
                    cmd.Parameters.AddWithValue("@phone", owner.Phone);
                    cmd.Parameters.AddWithValue("@neighborhoodId", owner.NeighborhoodId);
                    cmd.Parameters.AddWithValue("@id", owner.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteOwner(int ownerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Owner
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", ownerId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}

