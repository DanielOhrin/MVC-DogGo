using System;
using System.Collections.Generic;

namespace DogGo.Models
{
    public class Walk
    {
        private TimeSpan _ts => TimeSpan.FromSeconds(Duration);
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int WalkerId { get; set; }
        public Walker Walker { get; set; }
        public int DogId { get; set; }
        public Dog Dog { get; set; }
        public string MinuteString => _ts.ToString(@"mm' min'");
        public List<Dog> Dogs { get; set; }
    }
}
