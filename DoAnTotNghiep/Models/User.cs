using Microsoft.AspNetCore.Identity;

namespace DoAnTotNghiep.Models
{
    public class User: IdentityUser
    {
        public string? AccoutType { get; set; }
        public bool? IsLocked { get; set; }
    }
}
