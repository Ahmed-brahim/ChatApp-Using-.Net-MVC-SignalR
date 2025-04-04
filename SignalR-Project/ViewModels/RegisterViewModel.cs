using System.ComponentModel.DataAnnotations;

namespace SignalR_Project.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="First Name is Required")]
        [MinLength(3, ErrorMessage = "At least 3 Characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        public string? Country { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
