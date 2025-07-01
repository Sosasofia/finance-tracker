namespace FinanceTracker.Server.Models.DTOs.Response
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public UserDTO User { get; set; }
    }
}
