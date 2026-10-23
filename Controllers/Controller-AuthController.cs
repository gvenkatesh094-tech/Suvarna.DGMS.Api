using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Suvarna.DGMS.Api.Helpers;
using Suvarna.DGMS.Api.Models;
using Suvarna.DGMS.Api.Services;

namespace Suvarna.DGMS.Api.Controllers
{
    [Authorize]
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _service;
        private readonly SessionContext _session;
        private readonly PropertyAccessHelper _access;

        public AuthController(
            AuthService service,
            SessionContext session,
            PropertyAccessHelper access)
        {
            _service = service;
            _session = session;
            _access = access;
        }

        // =====================================
        // PROPERTY LEVEL ACCESS
        // =====================================

        /// <summary>Properties of one location for the login screen (user taken from the JWT).</summary>
        [HttpGet("login-properties")]
        public async Task<IActionResult> GetLoginProperties([FromQuery] long locId, [FromQuery] long userId = 0)
        {
            long jwtUserId = _access.JwtUserId;

            return Ok(await _service.GetLoginProperties(jwtUserId > 0 ? jwtUserId : userId, locId));
        }

        /// <summary>Change the property of the current session (header X-Session-Id).</summary>
        [HttpPost("switch-property")]
        public async Task<IActionResult> SwitchProperty([FromBody] SwitchPropertyRequest request)
        {
            return Ok(await _service.SwitchProperty(_session.SessionId ?? string.Empty, request?.PropertyId ?? -1));
        }

        [AllowAnonymous]
        [HttpGet("client-name")]
        public async Task<IActionResult> GetClientName()
        {
            return Ok(
                await _service.GetClientName());
        }
        // =====================================
        // PUBLIC LOGIN DOESNOT NEEDED AUTHORISATION 
        // =====================================

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response =
                await _service.Login(request);

            return Ok(response);
        }
        [HttpGet("user-details")]
        public async Task<IActionResult> GetUserDetails([FromQuery] long userId)
        {
            return Ok(
                await _service.GetUserDetails(userId));
        }

        [HttpGet("get-user-facilities")]
        public async Task<IActionResult> GetUserFacilities([FromQuery] long userId)
        {
            var response = await _service.GetUserFacilities(userId);
            return Ok(response);
        }
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateSession([FromBody] UserSessionRequest request)
        {
            //request.Terminal = HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;


            //var response =
            //    await _service.CreateSession(
            //        request);

            //return Ok(response);

            // =====================================
            // SERVER MACHINE
            // =====================================

            request.Machine =
                Environment.MachineName;

            // =====================================
            // WINDOWS VERSION
            // =====================================

            request.Version =
                Environment.OSVersion.ToString();

            // =====================================
            // CLIENT IP
            // =====================================

            request.Terminal =
                HttpContext.Connection
                    .RemoteIpAddress
                    ?.ToString() ?? string.Empty;

            // =====================================
            // IIS / WINDOWS USER
            // =====================================

            request.OsUser =
                Environment.UserName;

            //======================================
            // GET CLIENT NAME
            //======================================

            request.ClientName = "FNB";

            // =====================================
            // CREATE SESSION
            // =====================================

            var response = await _service.CreateSession(request);
            return Ok(response);
        }

        [AllowAnonymous]

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var response = await _service.RefreshToken(request);

                if (!response.SuccessStatus)
                {
                    return Unauthorized(
                        response);
                }

                return Ok(response);
            }
            catch (Exception)
            {
                // FIX (security): no internal error text to the browser; GlobalExceptionMiddleware logs it with a reference id
                throw;
            }
        }
        [AllowAnonymous]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            var response = await _service.Logout(request);

            return Ok(response);
        }
        [HttpPost("get-modules")]
        public async Task<IActionResult> GetModules([FromBody] ModuleRequest request)
        {
            var response = await _service.GetModules(request);
            return Ok(response);
        }

        [HttpPost("get-user-module-docs")]
        public async Task<IActionResult> GetUserModuleDocs([FromBody] ModuleDocRequest request)
        {
            var response = await _service.GetUserModuleDocs(request);

            return Ok(response);
        }

        //[HttpGet("get-users-by-location")]
        //public async Task<IActionResult>GetUsersByLocation([FromQuery] long locId)
        //{
        //    var response =await _service.GetUsersByLocation(locId);

        //    return Ok(response);
        //}
    }
}
