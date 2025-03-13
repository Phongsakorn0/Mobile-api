using System;
using System.Collections.Generic;

namespace ToDo.DTOs
{
    public class Login
    {
        public required string Id { get; set; }
        public required string Password { get; set; }
    }
}   