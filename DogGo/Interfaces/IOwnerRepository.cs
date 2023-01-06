using System.Collections.Generic;

using DogGo.Models;

namespace DogGo.Interfaces
{
    public interface IOwnerRepository
    {
        public List<Owner> GetAllOwners();
        public Owner GetByOwnerId(int id);
        public void DeleteOwner(int ownerId);
        public void UpdateOwner(Owner owner);
        public void AddOwner(Owner owner);
        public Owner GetOwnerByEmail(string email);
    }
}
