// TODOlist.Data/UserRegisterDto.cs
using System.ComponentModel.DataAnnotations;

namespace TODOlist.Data
{
    public class UserRegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // Include other properties if needed, e.g., ConfirmPassword
    }
}
