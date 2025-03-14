namespace ToDo.DTOs
{
    public class Register
    {
        public required string NationalId { get; set; }
        public required string Password { get; set; }
        public required string Tittle { get; set; }
        public required string Firstname { get; set; }
        public required string Lastname { get; set; }
    }
}