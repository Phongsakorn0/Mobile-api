using System;
using System.Collections.Generic;

namespace ToDo.Models
{
    public partial class User
    {
        public User()
        {
            Activity = new HashSet<Activity>();
        }

        public int Id { get; set; }
        public string HashedPassword { get; set; } = null!;
        public string Salt { get; set; } = null!;
        public string NationalId { get; set; } = null!;
        public string Tittle { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public string Lastname { get; set; } = null!;

        public virtual ICollection<Activity> Activity { get; set; }
    }
}
