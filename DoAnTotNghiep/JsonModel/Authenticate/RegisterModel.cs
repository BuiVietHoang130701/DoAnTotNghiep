using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.JsonModel.Authenticate
{
    public class RegisterModel
    {
        public string FullName { get; set; }
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        [Phone]
        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Authenticate type is required")]
        public string AccoutType { get; set; }
        public bool IsLocked { get; set; }
    }
}
