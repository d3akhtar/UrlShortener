using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using UrlShortener.Data;
using UrlShortener.Model;
using UrlShortener.Model.DTO;
using UrlShortener.Utilities;
using static QRCoder.PayloadGenerator;

namespace UrlShortener.Controllers
{
    [Route("api/alias")]
    [ApiController]
    public class AliasController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private ApiResponse _response;
        public AliasController(ApplicationDbContext db)
        {
            _db = db;
            _response = new ApiResponse();
        }

        [Authorize(Roles=GlobalConstants.ADMIN_ROLE)]
        [HttpGet]
        public async Task<ActionResult<ApiResponse>> GetAllAliases(string searchQuery = "", int pageNumber = 1, int pageSize = 10)
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
            List<Alias> aliases =
                _db.Aliases.Where(x =>
                x.Code.ToLower().Contains(searchQuery.ToLower()) ||
                x.Url.Contains(searchQuery.ToLower())).
                Skip((pageNumber - 1) * pageSize).
                Take(pageSize).
                ToList();

            Pagination pagination = new()
            {
                PageSize = pageSize,
                TotalRecords = aliases.Count(),
                CurrentPageNumber = pageNumber,
            };

            _response.IsSuccess = true;
            _response.Result = new
            {
                aliases = aliases,
                pagination = pagination
            };
            _response.RedirectUrl = null;
            _response.StatusCode = HttpStatusCode.OK;
            return Ok(_response);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse>> AddAlias([FromBody]AliasUpsertDTO aliasCreateDTO)
        {
            string url = aliasCreateDTO.Url;
            string code = aliasCreateDTO.Code;
            try
            {
                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "Url format is incorrect!" };
                    return BadRequest(_response);
                }
                Alias? alias = _db.Aliases.FirstOrDefault(u => u.Code == code);
                if (alias != null)
                {
                    _response.IsSuccess = false;
                    _response.Result = null;
                    _response.RedirectUrl = null;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "Alias has been taken!" };
                    return BadRequest(_response);
                }
                else
                {
                    alias = new()
                    {
                        Code = code,
                        Url = url
                    };

                    string shortenedUrl = Helpers.GetBaseUrlWithRequest(HttpContext.Request).Replace("alias","url") + alias.Code;

                    alias.PngQrCodeImage = Convert.ToBase64String(Helpers.GetPngQrCodeImage(shortenedUrl));
                    alias.SvgQrCodeImage = Helpers.GetSvgQrCodeImage(shortenedUrl);
                    alias.AsciiQrCodeRepresentation = Helpers.GetAsciiQrCodeRepresentation(shortenedUrl);

                    _db.Aliases.Add(alias);
                    _db.SaveChanges();

                    _response.IsSuccess = true;
                    _response.Result = alias;
                    _response.RedirectUrl = shortenedUrl;
                    _response.StatusCode = HttpStatusCode.Created;
                    return Ok(_response);
                }
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { "Some unknown error has occured." };
                return BadRequest(_response);
            }
        }

        [HttpDelete("{code}")]
        [Authorize(Roles = GlobalConstants.ADMIN_ROLE)]
        public async Task<ActionResult<ApiResponse>> DeleteAlias(string code)
        {
            Alias? alias = _db.Aliases.FirstOrDefault(x => x.Code == code);
            if (alias == null)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string> { "Alias wasn't found!" };
                return NotFound(_response);
            }
            else
            {
                _db.Aliases.Remove(alias);
                _db.SaveChanges();

                _response.IsSuccess = true;
                _response.Result = $"Alias {alias.Code} was deleted successfully!";
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }

        [HttpPut("{code}")]
        [Authorize(Roles = GlobalConstants.ADMIN_ROLE)]
        public async Task<ActionResult<ApiResponse>> UpdateAlias(string code, [FromBody]AliasUpsertDTO aliasUpsertDTO)
        {
            if (!Uri.IsWellFormedUriString(aliasUpsertDTO.Url, UriKind.Absolute))
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.ErrorMessages = new List<string> { "Url format is incorrect!" };
                return BadRequest(_response);
            }
            Alias? alias = _db.Aliases.FirstOrDefault(x => x.Code == code);
            if (alias == null)
            {
                _response.IsSuccess = false;
                _response.Result = null;
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.NotFound;
                _response.ErrorMessages = new List<string> { "Alias wasn't found!" };
                return NotFound(_response);
            }
            else
            {
                if (aliasUpsertDTO.Code != alias.Code && !string.IsNullOrEmpty(aliasUpsertDTO.Code))
                {
                    bool isAliasTaken = _db.Aliases.FirstOrDefault(x => x.Code == aliasUpsertDTO.Code) != null;
                    if (isAliasTaken)
                    {
                        _response.IsSuccess = false;
                        _response.Result = null;
                        _response.RedirectUrl = null;
                        _response.StatusCode = HttpStatusCode.BadRequest;
                        _response.ErrorMessages = new List<string> { "Alias has been taken!" };
                        return BadRequest(_response);
                    }
                    else
                    {
                        List<ApplicationUserAlias> applicationUserAliases = 
                            _db.ApplicationUserAliases.Where(x => x.AliasCode == alias.Code).ToList();
                        foreach (var applicationUserAlias in applicationUserAliases)
                        {
                            applicationUserAlias.AliasCode = aliasUpsertDTO.Code;
                            _db.ApplicationUserAliases.Update(applicationUserAlias);
                        }

                        _db.Aliases.Remove(alias);
                        _db.Aliases.Add(new Alias
                        {
                            Url = aliasUpsertDTO.Url,
                            Code = aliasUpsertDTO.Code,
                            PngQrCodeImage = Convert.ToBase64String(Helpers.GetPngQrCodeImage(Helpers.GetBaseUrlWithRequest(HttpContext.Request).Replace("alias", "url") + aliasUpsertDTO.Code)),
                            SvgQrCodeImage = Helpers.GetSvgQrCodeImage(Helpers.GetBaseUrlWithRequest(HttpContext.Request).Replace("alias", "url") + aliasUpsertDTO.Code),
                            AsciiQrCodeRepresentation = Helpers.GetAsciiQrCodeRepresentation(Helpers.GetBaseUrlWithRequest(HttpContext.Request).Replace("alias", "url") + aliasUpsertDTO.Code),
                        });
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(aliasUpsertDTO.Url))
                    {
                        alias.Url = aliasUpsertDTO.Url;
                        _db.Aliases.Update(alias);
                    }
                    
                }
                _db.SaveChanges();

                _response.IsSuccess = true;
                _response.Result = $"Alias {alias.Code} was updated successfully!";
                _response.RedirectUrl = null;
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
        }
    }
}
