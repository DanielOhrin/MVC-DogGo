using System.Collections.Generic;

using DogGo.Models;

namespace DogGo.Interfaces
{
    public interface IWalkRepository
    {
        List<Walk> GetWalksByWalkerId(int walkerId);
        void CreateWalk(Walk walk);
    }
}
