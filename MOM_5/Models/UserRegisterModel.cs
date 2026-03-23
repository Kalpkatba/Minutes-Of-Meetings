using System.ComponentModel.DataAnnotations;

namespace MOM_5.Models
{
    public class UserRegisterModel
    {
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Password must be between 4 and 50 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Mobile No is required.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit mobile number.")]
        public string MobileNo { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }

        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters.")]
        public string? Address { get; set; }
    }
}
