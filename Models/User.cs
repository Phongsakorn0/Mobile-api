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

        public string Id { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Salt { get; set; } = null!;

        public virtual ICollection<Activity> Activity { get; set; }
    }
}
