using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Net;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Utilities;

namespace UrlShortener.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserUrlsController : ControllerBase
    {
        private ApiResponse _response;
        private UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _db;

        public ApplicationUserUrlsController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _response = new ApiResponse();
            _userManager = userManager;
            _db = db;
        }

        [Authorize(Roles = GlobalConstants.ADMIN_ROLE)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllUrlsAndAliasesWithUsers
            (string searchQuery = "", int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(searchQuery)) searchQuery = "";
            if (pageSize > 100) pageSize = 100;
            if (pageNumber < 1)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.ErrorMessages = new List<string>() { "Page number must be greater than 0!" };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            List<ApplicationUserUrlCode> urlCodes = _db.ApplicationUserUrlCodes.ToList();
            List<ApplicationUserAlias> aliases = _db.ApplicationUserAliases.ToList();
            
            List<object> res = new List<object>();
            res.AddRange(urlCodes.Select(x => new
            {
                userId = x.ApplicationUserId,
                urlCode = x.UrlCode
            }));
            res.AddRange(aliases.Select(x => new
            {
                userId = x.ApplicationUserId,
                alias = x.Alias
            }));

            _response.IsSuccess = true;
            _response.Result = res.Skip((pageNumber - 1) * pageSize).Take(pageSize);
            _response.RedirectUrl = null;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [Authorize]
        [HttpGet("{userId}")]
        public async Task<ActionResult<ApiResponse>> GetUrlsAndAliasesForUser
            (string userId, string searchQuery = "", int pageNumber = 1, int pageSize = 10)
        {
            if (string.IsNullOrEmpty(searchQuery)) searchQuery = "";
            if (pageSize > 100) pageSize = 100;
            if (pageNumber < 1)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.ErrorMessages = new List<string>() { "Page number must be greater than 0!" };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            bool userExists = await _userManager.FindByIdAsync(userId) != null;
            if (!userExists)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string> { "User not found!" };
                return NotFound(_response);
            }
            else
            {
                List<ApplicationUserUrlCode> urlCodes =
                    _db.ApplicationUserUrlCodes.
                    Where(x => x.ApplicationUserId == userId && 
                         (x.UrlCode.Code.ToLower().Contains(searchQuery.ToLower()) || 
                         x.UrlCode.Url.ToLower().Contains(searchQuery.ToLower()))).
                         ToList();

                List<ApplicationUserAlias> aliases =
                    _db.ApplicationUserAliases.
                    Where(x => x.ApplicationUserId == userId &&
                         (x.Alias.Code.ToLower().Contains(searchQuery.ToLower()) ||
                         x.Alias.Url.ToLower().Contains(searchQuery.ToLower()))).
                         ToList();

                List<object> res = new List<object>();
                res.AddRange(urlCodes.Select(x => new
                {
                    userId = x.ApplicationUserId,
                    urlCode = x.UrlCode
                }));
                res.AddRange(aliases.Select(x => new
                {
                    userId = x.ApplicationUserId,
                    alias = x.Alias
                }));

                Pagination pagination = new()
                {
                    PageSize = pageSize,
                    TotalRecords = res.Count(),
                    CurrentPageNumber = pageNumber,
                };

                _response.IsSuccess = true;
                _response.Result = new
                {
                    userUrlCodes = res.Skip((pageNumber - 1) * pageSize).Take(pageSize),
                    pagination = pagination
                };
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }

        [Authorize]
        [HttpPost("{userId}")]
        public async Task<ActionResult<ApiResponse>> AddUrlCodeToUser(string userId, List<string> codes)
        {
            try
            {
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "User wasn't found!" };
                    return NotFound(_response);
                }
                else
                {
                    foreach (string code in codes)
                    {
                        UrlCode? urlCode = _db.UrlCodes.FirstOrDefault(u => u.Code == code);
                        if (urlCode == null)
                        {
                            Alias? alias = _db.Aliases.FirstOrDefault(u => u.Code == code);
                            if (alias == null)
                            {
                                _response.IsSuccess = false;
                                _response.Result = null;
                                _response.RedirectUrl = null;
                                _response.StatusCode = HttpStatusCode.NotFound;
                                _response.ErrorMessages = new List<string> { $"No url mapped to the code: {code}" };
                                return NotFound(_response);
                            }
                            else
                            {
                                ApplicationUserAlias? applicationUserAlias =
                                    _db.ApplicationUserAliases.FirstOrDefault
                                    (u => u.ApplicationUserId == user.Id && u.AliasCode == alias.Code);

                                if (applicationUserAlias != null)
                                {
                                    _response.IsSuccess = false;
                                    _response.Result = null;
                                    _response.RedirectUrl = null;
                                    _response.StatusCode = HttpStatusCode.BadRequest;
                                    _response.ErrorMessages = new List<string> { $"User already has the alias: {code}" };
                                    return BadRequest(_response);
                                }

                                applicationUserAlias = new()
                                {
                                    AliasCode = alias.Code,
                                    ApplicationUserId = user.Id,
                                };
                                _db.ApplicationUserAliases.Add(applicationUserAlias);
                            }
                        }
                        else
                        {
                            ApplicationUserUrlCode? applicationUserUrlCode =
                                    _db.ApplicationUserUrlCodes.FirstOrDefault
                                    (u => u.ApplicationUserId == user.Id && u.UrlCodeCode == urlCode.Code);

                            if (applicationUserUrlCode != null)
                            {
                                _response.IsSuccess = false;
                                _response.Result = null;
                                _response.RedirectUrl = null;
                                _response.StatusCode = HttpStatusCode.BadRequest;
                                _response.ErrorMessages = new List<string> { $"User already has the urlCode: {code}" };
                                return BadRequest(_response);
                            }

                            applicationUserUrlCode = new()
                            {
                                UrlCodeCode = urlCode.Code,
                                ApplicationUserId = user.Id,
                            };
                            _db.ApplicationUserUrlCodes.Add(applicationUserUrlCode);
                        }
                    }
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.Result = "Added all codes successfully!";
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
            }
            catch (Exception e)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { e.Message };
                return BadRequest(_response);
            }
        }

        [Authorize]
        [HttpDelete("{userId}")]
        public async Task<ActionResult<ApiResponse>> DeleteUserUrl(string userId, string code)
        {
            try
            {
                ApplicationUser? user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "User wasn't found!" };
                    return NotFound(_response);
                }

                ApplicationUserUrlCode? userUrlCode = _db.ApplicationUserUrlCodes.
                    FirstOrDefault(x => x.ApplicationUserId == userId && x.UrlCodeCode == code);

                if (userUrlCode == null)
                {
                    ApplicationUserAlias? userAlias = _db.ApplicationUserAliases.
                        FirstOrDefault(x => x.ApplicationUserId == userId && x.AliasCode == code);
                    if (userAlias == null)
                    {
                        _response.IsSuccess = false;
                        _response.Result = null;
                        _response.RedirectUrl = null;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.ErrorMessages = new List<string> { "User doesn't have a url with that code." };
                        return NotFound(_response);
                    }
                    else
                    {
                        _db.ApplicationUserAliases.Remove(userAlias);
                        _db.SaveChanges();

                        _response.IsSuccess = true;
                        _response.Result = $"Code {code} for user with id {userId} was deleted successfully!";
                        _response.RedirectUrl = null;
                        _response.StatusCode = HttpStatusCode.OK;
                        return Ok(_response);
                    }
                }
                else
                {
                    _db.ApplicationUserUrlCodes.Remove(userUrlCode);
                    _db.SaveChanges();

                    _response.IsSuccess = true;
                    _response.Result = $"Code {code} for user with id {userId} was deleted successfully!";
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }

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
    }
}
