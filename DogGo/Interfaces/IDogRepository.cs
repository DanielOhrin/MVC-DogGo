using System.Collections.Generic;

using DogGo.Models;

namespace DogGo.Interfaces
{
    public interface IDogRepository
    {
        public List<Dog> GetAllDogs();
        public void AddDog(Dog dog);
        public void DeleteDog(Dog dog);
        public void UpdateDog(Dog dog);
        public Dog GetById(int id);
        public List<Dog> GetDogsByOwnerId(int ownerId);
    }
}
