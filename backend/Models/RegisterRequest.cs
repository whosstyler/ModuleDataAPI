namespace backend.Models
{
    public class RegisterRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string HWID { get; set; }
        public string Avatar { get; set; } // Optional
    }
}
