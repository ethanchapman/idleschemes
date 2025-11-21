using IdleSchemes.Api.Models.Input;
using IdleSchemes.Core.Models;
using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase {

        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;
        private readonly UserSessionService _userSessionService;
        private readonly IdleDbContext _dbContext;
        private readonly TimeService _timeService;

        public UserController(ILogger<UserController> logger,
            UserService userService, 
            UserSessionService userSessionService, 
            IdleDbContext dbContext,
            TimeService timeService) {
            _logger = logger;
            _userService = userService;
            _userSessionService = userSessionService;
            _dbContext = dbContext;
            _timeService = timeService;
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginModel model) {
            if (_userSessionService.CurrentUser is not null) {
                return BadRequest();
            }
            var session = await _userSessionService.TryLoginAsync(model.UserLogin, model.Password);
            if (session is null) {
                return BadRequest();
            }
            return Ok(session.Id);
        }

        [HttpPost("logout", Name = "Logout")]
        public async Task<ActionResult> Logout() {
            await _userSessionService.VoidSessionAsync();
            return Ok();
        }

        [HttpPost("register", Name = "Register")]
        public async Task<ActionResult<UserModel>> Register([FromBody] RegisterUserModel model) {
            var user = await _userService.GetOrCreateUserAsync(email: model.Email, phone: model.Phone, name: model.Name);
            if (!string.IsNullOrEmpty(user.Password)) {
                _logger.LogInformation("User attempted to register an email/phone that already has a password: {Email}/{Phone}",
                    model.Email, model.Phone);
                return BadRequest();
            }
            _userService.SetPassword(user, model.Password);
            return Ok(new UserModel(user));
        }

        [HttpGet("me", Name = "GetMyUserInfo")]
        public ActionResult<UserModel> GetMyUserInfo([FromBody] RegisterUserModel model) {
            if(_userSessionService.CurrentUser is null) {
                return Unauthorized();
            }
            return Ok(new UserModel(_userSessionService.CurrentUser));
        }

        [HttpPut("me", Name = "UpdateMyUserInfo")]
        public async Task<ActionResult<UserModel>> UpdateMyUserInfo([FromBody] UpdateUserModel model) {
            var user = _userSessionService.CurrentUser;
            if (user is null) {
                return Unauthorized();
            }

            if (model.RemoveEmail) {
                user.Email = null;
                user.EmailVerified = null;
            } else if (!string.IsNullOrEmpty(model.Email)) {
                user.Email = model.Email;
                user.EmailVerified = null;
            }
            if (model.RemovePhone) {
                user.Phone = null;
                user.PhoneVerified = null;
            } else if (_userService.IsPhoneNumber(model.Phone, out var normalizedPhone)) {
                user.Phone = normalizedPhone;
                user.PhoneVerified = null;
            }

            if (!string.IsNullOrEmpty(model.Region)) {
                user.Region = await _dbContext.Regions
                    .SingleOrDefaultAsync(r => r.Id == model.Region);
                if (user.Region is not null && !string.IsNullOrEmpty(user.TimeZone)) {
                    user.TimeZone = user.Region?.TimeZone;
                }
            }
            if (!string.IsNullOrEmpty(model.TimeZone)) {
                user.TimeZone = TimeZoneInfo.GetSystemTimeZones()
                    .FirstOrDefault(tz => tz.Id == model.TimeZone)?.Id;
            }
            if (!string.IsNullOrEmpty(model.Password)) {
                _userService.SetPassword(user, model.Password);
            }

            await _dbContext.SaveChangesAsync();
            return Ok(new UserModel(user));
        }

    }
}
