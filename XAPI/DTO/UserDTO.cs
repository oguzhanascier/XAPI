using System.ComponentModel.DataAnnotations;

namespace XAPI.DTO
{
    public class UserDTO
    {
        [Required]
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}