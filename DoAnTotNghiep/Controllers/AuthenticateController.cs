
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.Drawing;
using System.Drawing.Imaging;
using DoAnTotNghiep.Interfaces;
using static DoAnTotNghiep.JsonModel.Response;
using DoAnTotNghiep.JsonModel.Authenticate;

namespace DoAnTotNghiep.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private IAuthenticate service;

        public AuthenticateController(IAuthenticate service)
        {
            this.service = service;
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] Login_model model)
        {
            ResponseAPI<JsonLogin> responseAPI = new ResponseAPI<JsonLogin>();
            try
            {
                responseAPI.Data = await this.service.login(model);
                return Ok(responseAPI.Data);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
       
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            ResponseAPI<string> responseAPI = new ResponseAPI<string>();
            try
            {
                responseAPI.Data = await this.service.Register(model);
                return Ok(responseAPI.Data);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string username)
        {
            ResponseAPI<string> responseAPI = new ResponseAPI<string>();
            try
            {
                responseAPI.Data = await this.service.ForgotPassword(username);
                return Ok(responseAPI.Data);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string username, string resetToken)
        {
            ResponseAPI<string> responseAPI = new ResponseAPI<string>();
            try
            {
                responseAPI.Data = await this.service.ResetPassword(username, resetToken);
                return Ok(responseAPI.Data);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        [HttpPost]
        [Route("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string Username, string newPass)
        {
            ResponseAPI<string> responseAPI = new ResponseAPI<string>();
            try
            {
                responseAPI.Data = await this.service.ChangePassword(Username, newPass);
                return Ok(responseAPI.Data);
            }
            catch (Exception ex)
            {
                responseAPI.Message = ex.Message;
                return BadRequest(responseAPI);
            }
        }
        
    }
}
