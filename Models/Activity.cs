using System;
using System.Collections.Generic;

namespace ToDo.Models
{
    public partial class Activity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime When { get; set; }
        public int Userid { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
