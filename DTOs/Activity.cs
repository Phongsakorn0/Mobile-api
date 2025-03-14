using System;
using System.Collections.Generic;

namespace ToDo.DTOs
{
    public class Activity
    {
        public required string Name { get; set; }
        public DateTime When { get; set; }
    }
}