using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Utilities;

namespace UrlShortener.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private ApiResponse _response;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        private readonly string secretKey;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db, IConfiguration configuration)
        {
            _response = new ApiResponse();
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse>> Login([FromBody]LoginRequest loginRequest)
        {
            LoginResponse loginResponse = new LoginResponse();
            if (ModelState.IsValid)
            {
                ApplicationUser? user = await _userManager.FindByEmailAsync(loginRequest.Email);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "This email has not been registered yet!" };
                    return NotFound(_response);
                }
                else
                {
                    //string hashedPassword = _userManager.PasswordHasher.HashPassword(user, loginRequest.Password);
                    bool passwordMatch = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
                    if (passwordMatch)
                    {
                        var userRole = await _userManager.GetRolesAsync(user);

                        JwtSecurityTokenHandler tokenHandler = new();
                        byte[] key = Encoding.ASCII.GetBytes(secretKey);

                        SecurityTokenDescriptor descriptor = new()
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                            {
                                new Claim("id", user.Id),
                                new Claim("email", user.Email),
                                new Claim("username", user.UserName),
                                new Claim("role", userRole.FirstOrDefault()),
                            }),
                            Expires = DateTime.UtcNow.AddDays(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                        };

                        SecurityToken token = tokenHandler.CreateToken(descriptor);

                        loginResponse.Token = tokenHandler.WriteToken(token);
                        _response.IsSuccess = true;
                        _response.Result = loginResponse;
                        _response.RedirectUrl = null;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.ErrorMessages = null;
                        return Ok(_response);
                    }
                    else
                    {
                        _response.IsSuccess = false;
                        _response.Result = null;
                        _response.RedirectUrl = null;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages = new List<string> { "Incorrect password!" };
                        return BadRequest(_response);
                    }
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { "Invalid login request!" };

                return BadRequest(_response);
            }
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse>> Register([FromBody]RegisterRequest registerRequest)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = registerRequest.Username,
                        Email = registerRequest.Email,
                    };
                    await _userManager.CreateAsync(user, registerRequest.Password);

                    bool roleExists = await _roleManager.RoleExistsAsync(registerRequest.Role);

                    if (!roleExists)
                    {
                        switch (registerRequest.Role)
                        {
                            case GlobalConstants.ADMIN_ROLE: 
                                await _roleManager.CreateAsync(new IdentityRole(GlobalConstants.ADMIN_ROLE)); break;
                            case GlobalConstants.USER_ROLE:
                                await _roleManager.CreateAsync(new IdentityRole(GlobalConstants.USER_ROLE)); break;
                            default:
                                _response.IsSuccess = false;
                                _response.Result = null;
                                _response.RedirectUrl = null;
                                _response.StatusCode = HttpStatusCode.BadRequest;
                                _response.ErrorMessages = new List<string> { "Invalid role, choose between admin or user" };
                                return BadRequest(_response);
                        } 
                    }

                    await _userManager.AddToRoleAsync(user, registerRequest.Role);

                    //_db.ApplicationUsers.Add(user);
                    //_db.SaveChanges();

                    _response.IsSuccess = true;
                    _response.Result = "User created successfully";
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.OK;

                    return Ok(_response);
                }
                catch(Exception e)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { e.Message };

                    return BadRequest(_response);
                }
            }
            else
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { "Invalid register request!" };

                return BadRequest(_response);
            }
        }
    }
}
