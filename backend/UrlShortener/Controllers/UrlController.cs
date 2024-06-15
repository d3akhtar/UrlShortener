using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Utilities;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using QRCoder;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Runtime.Intrinsics.X86;


namespace UrlShortener.Controllers
{
    [Route("api/url")]
    [ApiController]
    public class UrlController : ControllerBase
    {
        private readonly ApiResponse _response;
        private readonly ApplicationDbContext _db;
        private UserManager<ApplicationUser> _userManager;
        public UrlController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _response = new ApiResponse();
            _db = db;
        }

        [Authorize(Roles = GlobalConstants.ADMIN_ROLE)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetUrls(string searchQuery = "", int pageNumber = 1, int pageSize = 10)
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
            List<UrlCode> codes = 
                _db.UrlCodes.Where(x =>
                x.Code.ToLower().Contains(searchQuery.ToLower()) ||
                x.Url.Contains(searchQuery.ToLower())).
                Skip((pageNumber - 1) * pageSize).
                Take(pageSize).
                ToList();

            Pagination pagination = new()
            {
                PageSize = pageSize,
                TotalRecords = codes.Count(),
                CurrentPageNumber = pageNumber,
            };

            _response.IsSuccess = true;
            _response.Result = new
            {
                codes = codes,
                pagination = pagination
            };
            _response.RedirectUrl = null;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpGet("{code}", Name="GetUrl")]
        public async Task<ActionResult<ApiResponse>> GetUrl(string code)
        {
            try
            {
                UrlCode? urlCode = _db.UrlCodes.FirstOrDefault(x => x.Code == code);

                if (urlCode == null)
                {
                    Alias? alias = _db.Aliases.FirstOrDefault(x => x.Code == code);
                    if (alias == null)
                    {
                        _response.IsSuccess = false;
                        _response.Result = null;
                        _response.RedirectUrl = null;
                        _response.StatusCode = HttpStatusCode.NotFound;
                        _response.ErrorMessages = new List<string> { "No url mapped to this code" };
                        return NotFound(_response);
                    }
                    else
                    {
                        return new RedirectResult(alias.Url);
                    }
                }
                else
                {
                    return new RedirectResult(urlCode.Url);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { ex.Message };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> PostUrl([FromForm] string url)
        {
            string shortenedUrl;
            try
            {
                if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    UrlCode? urlCode = _db.UrlCodes.FirstOrDefault(x => x.Url == url);
                    if (urlCode != null)
                    {
                        shortenedUrl = Helpers.GetBaseUrlWithRequest(HttpContext.Request) + urlCode.Code;
                        _response.IsSuccess = true;
                        _response.Result = urlCode;
                        _response.RedirectUrl = shortenedUrl;
                        _response.StatusCode = HttpStatusCode.OK;
                        return Ok(_response);
                    }
                    string key = GetKey(url);

                    UrlCodeKey? urlCodeKey = _db.UrlCodeKeys.FirstOrDefault(x => x.Key == key);

                    if (urlCodeKey == null)
                    {
                        urlCodeKey = new()
                        {
                            Key = key,
                            LastUsedValue = "aa"
                        };
                        _db.UrlCodeKeys.Add(urlCodeKey);
                        _db.SaveChanges();
                    }
                    else
                    {
                        List<UrlCode> urlCodesWithKey = _db.UrlCodes.Where(x => x.Code.Substring(0, 6) == urlCodeKey.Key).ToList();
                        urlCodeKey.LastUsedValue = GetNextValue(urlCodeKey.LastUsedValue);
                        while (urlCodesWithKey.FirstOrDefault(x => x.Code.Substring(6,2) == urlCodeKey.LastUsedValue) != null)
                        {
                            urlCodeKey.LastUsedValue = GetNextValue(urlCodeKey.LastUsedValue);
                        }
                        _db.UrlCodeKeys.Update(urlCodeKey);
                        _db.SaveChanges();
                    }

                    shortenedUrl = Helpers.GetBaseUrlWithRequest(HttpContext.Request) + urlCodeKey.Key + urlCodeKey.LastUsedValue;

                    urlCode = new UrlCode()
                    {
                        Code = urlCodeKey.Key + urlCodeKey.LastUsedValue,
                        Url = url,
                        PngQrCodeImage = Convert.ToBase64String(Helpers.GetPngQrCodeImage(shortenedUrl)),
                        SvgQrCodeImage = Helpers.GetSvgQrCodeImage(shortenedUrl),
                        AsciiQrCodeRepresentation = Helpers.GetAsciiQrCodeRepresentation(shortenedUrl),
                    };

                    _db.UrlCodes.Add(urlCode);
                    _db.SaveChanges();

                    _response.Result = urlCode;
                    _response.IsSuccess = true;
                    _response.RedirectUrl = shortenedUrl;
                    _response.StatusCode = HttpStatusCode.OK;
                    return Ok(_response);
                }
                else
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "Invalid url" };
                    return BadRequest(_response);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = ex.Message;
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = GlobalConstants.ADMIN_ROLE)]
        public async Task<ActionResult<ApiResponse>> DeleteUrl(string code)
        {
            try
            {
                UrlCode? urlCode = _db.UrlCodes.FirstOrDefault(x => x.Code == code);
                if (urlCode == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "No url mapped to this code" };
                    return NotFound(_response);
                }
                else
                {
                    _db.UrlCodes.Remove(urlCode);
                    _db.SaveChanges();

                    UrlCodeKey urlCodeKey = _db.UrlCodeKeys.First(x => x.Key == urlCode.Code.Substring(0, 6));
                    List<UrlCode> urlCodesWithKey = _db.UrlCodes.Where(x => x.Code.Substring(0, 6) == urlCodeKey.Key).ToList();
                    if (urlCodesWithKey == null || urlCodesWithKey.Count == 0)
                    {
                        _db.UrlCodeKeys.Remove(urlCodeKey);
                    }
                    else
                    {
                        urlCodeKey.LastUsedValue = urlCodesWithKey[urlCodesWithKey.Count - 1].Code.Substring(6, 2);
                        _db.UrlCodeKeys.Update(urlCodeKey);
                    }

                    _db.SaveChanges();

                    _response.IsSuccess = true;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.OK;
                    _response.ErrorMessages = new List<string> { "Url was deleted!" };
                    return Ok(_response);
                }
            }
            catch(Exception e)
            {
                _response.IsSuccess = false;
                _response.ErrorMessages = new List<string> { e.Message };
                _response.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }
        }

        [Authorize(Roles = GlobalConstants.ADMIN_ROLE)]
        [HttpPut("{code}")]
        public async Task<ActionResult<ApiResponse>> UpdateUrlCode(string code, string newUrl)
        {
            try
            {
                UrlCode? urlCode = _db.UrlCodes.FirstOrDefault(u => u.Code == code);
                if (urlCode == null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "Url with this code doesn't exist, it might be an alias." };
                    return NotFound(_response);
                }
                else
                {
                    urlCode.Url = newUrl;
                    _db.UrlCodes.Update(urlCode);
                    _db.SaveChanges();

                    _response.IsSuccess = true;
                    _response.Result = urlCode;
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


        private static string GetKey(string code)
        {
            Encoding encoding = Encoding.UTF8;
            var codeTextBytes = encoding.GetBytes(code);
            var convertedString = Convert.ToBase64String(codeTextBytes);
            return convertedString.Replace('+','p').Replace('/','s').TrimEnd('=').Substring((convertedString.Length - 9), 6);
            //return convertedString.Replace('+', 'p').Replace('/', 's').TrimEnd('=').Substring(0, 6); for testing
        }

        private static string GetNextValue(string value)
        {
            char first = value[0];
            char second = value[1];
            switch (second)
            {
                case 'z':
                    second = 'A';
                    break;
                case 'Z': 
                    second = '0'; 
                    break;
                case '9':
                    second = 'a';
                    switch (first)
                    {
                        case 'z':
                            first = 'A';
                            break;
                        case 'Z':
                            first = '0';
                            break;
                        case '9':
                            first = 'a';
                            second = 'a';
                            break;
                        default:
                            first++;
                            break;
                    }
                    break;
                default:
                    second++;
                    break;
            }
            return Char.ToString(first) + Char.ToString(second);
        }
    }
}
