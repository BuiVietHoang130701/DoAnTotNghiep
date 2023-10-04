using DoAnTotNghiep.JsonModel.Authenticate;

namespace DoAnTotNghiep.Interfaces
{
    public interface IAuthenticate
    {
        Task<JsonLogin> login(Login_model dto);
        Task<string> Register(RegisterModel model);
        Task<string> ForgotPassword(string username);
        Task<string> ResetPassword(string username, string resetToken);
        Task<string> ChangePassword(string Username, string newPass);
    }
}
