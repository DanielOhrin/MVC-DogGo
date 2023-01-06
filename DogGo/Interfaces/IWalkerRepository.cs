using System.Collections.Generic;

using DogGo.Models;

namespace DogGo.Interfaces
{
    public interface IWalkerRepository
    {
        List<Walker> GetAllWalkers();
        Walker GetWalkerById(int id);
        List<Walker> GetWalkersInNeighborhood(int neighborhoodId);

    }
}
