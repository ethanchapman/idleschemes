using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Api.Controllers {
    [ApiController]
    [Route("session")]
    public class AdminSessionController : ControllerBase {

        private readonly ILogger<AdminSessionController> _logger;
        private readonly UserSessionService _userSessionService;
        private readonly TimeService _timeService;
        private readonly IdleDbContext _dbContext;

        public AdminSessionController(ILogger<AdminSessionController> logger,
            UserSessionService userSessionService,
            TimeService timeService,
            IdleDbContext dbContext) {
            _logger = logger;
            _userSessionService = userSessionService;
            _timeService = timeService;
            _dbContext = dbContext;
        }

        [HttpPost("login", Name = "Login")]
        public async Task<ActionResult<string>> Login([FromForm] string userLogin, [FromForm] string userPassword) {
            if (_userSessionService.CurrentUser is not null) {
                return Redirect("/");
            }
            var session = await _userSessionService.TryLoginAsync(userLogin, userPassword);
            if (session is null) {
                return Redirect("/login");
            }
            var lastSession = await _dbContext.UserSessions
                .Include(s => s.ActiveAssociation)
                .FirstOrDefaultAsync(s => s.User == session.User && s.Id != session.Id);
            if(lastSession?.ActiveAssociation is not null) {
                session.ActiveAssociation = lastSession.ActiveAssociation;
            } else {
                session.ActiveAssociation = await _dbContext.Associates
                    .FirstOrDefaultAsync(a => a.User == session.User);
            }
            if (session.ActiveAssociation is not null) {
                await _dbContext.SaveChangesAsync();
            }
            HttpContext.Response.Cookies.Append(UserSessionService.SESSION_KEY, session.Id, new CookieOptions {
                Expires = _timeService.GetNow() + TimeSpan.FromHours(24)
            });
            return Redirect("/");
        }

        [HttpPost("logout", Name = "Logout")]
        public async Task<ActionResult> Logout() {
            await _userSessionService.VoidSessionAsync();
            return Ok();
        }

    }
}
