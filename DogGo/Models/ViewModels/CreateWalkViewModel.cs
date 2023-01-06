using System.Collections.Generic;

namespace DogGo.Models.ViewModels
{
    public class CreateWalkViewModel
    {
        public List<Dog> Dogs { get; set; }
        public List<Walker> Walkers { get; set; }
        public Walk Walk { get; set; } = new();
        public List<string> DogIds { get; set; }
    }
}
