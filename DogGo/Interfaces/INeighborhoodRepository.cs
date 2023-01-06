using System.Collections.Generic;

using DogGo.Models;

namespace DogGo.Interfaces
{
    public interface INeighborhoodRepository
    {
        List<Neighborhood> GetAll();
    }
}
