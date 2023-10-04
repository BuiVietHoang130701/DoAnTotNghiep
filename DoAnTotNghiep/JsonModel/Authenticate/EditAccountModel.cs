using System.ComponentModel.DataAnnotations;

namespace DoAnTotNghiep.JsonModel.Authenticate
{
    public class EditAuthenticateModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Fullname { get; set; }
        public int idDepartment { get; set; }
        public bool IsLocked { get; set; }
        [Phone]
        [Required(ErrorMessage = "Phone number is required")]
        public string Phone { get; set; }
    }
}
