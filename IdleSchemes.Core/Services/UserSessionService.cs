using IdleSchemes.Data;
using IdleSchemes.Data.Models;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Core.Services {
    public class UserSessionService {

        public static readonly string SESSION_KEY = "is_ssid";

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly TimeService _timeService;
        private readonly UserService _userService;

        public UserSessionService(IdleDbContext dbContext, IdService idService, TimeService timeService, UserService userService) {
            _dbContext = dbContext;
            _idService = idService;
            _timeService = timeService;
            _userService = userService;
        }

        public UserSession? CurrentSession { get; private set; }
        public User? CurrentUser => CurrentSession?.User;

        public event EventHandler? CurrentSessionUpdated;

        public void SetCurrentSesion(UserSession? userSession) {
            CurrentSession = userSession;
        }

        public async Task<UserSession?> TryLoginAsync(string userLogin, string password) {
            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Email == userLogin || u.Phone == userLogin);
            if (user == null) {
                return null;
            }
            if (!_userService.IsPasswordValid(user, password)) {
                return null;
            }
            return await CreateSessionForUserAsync(user);
        }

        public async Task<UserSession> CreateSessionForUserAsync(User user) {
            CurrentSession = _dbContext.UserSessions.Add(new UserSession {
                Id = _idService.GenerateId() + _idService.GenerateSecret(64),
                Created = _timeService.GetNow(),
                User = user
            }).Entity;
            await _dbContext.SaveChangesAsync();
            CurrentSessionUpdated?.Invoke(this, EventArgs.Empty);
            return CurrentSession;
        }

        public async Task<UserSession?> TryResumeSessionAsync(HttpContext httpContext, bool includeActiveAssociate = false, bool force = false) {
            var sessionId = httpContext.Request.Cookies[SESSION_KEY];
            if (string.IsNullOrEmpty(sessionId)) {
                return null;
            }
            return await TryResumeSessionAsync(sessionId, includeActiveAssociate, force);
        }

        public async Task<UserSession?> TryResumeSessionAsync(string sessionId, bool includeActiveAssociate = false, bool force = false) {
            if (CurrentSession is not null && !force) {
                return null;
            }
            IQueryable<UserSession> query = _dbContext.UserSessions
                .Include(s => s.User)
                .ThenInclude(u => u!.Region);
            if(includeActiveAssociate) {
                query = query
                    .Include(s => s.ActiveAssociation)
                    .ThenInclude(a => a!.Organization)
                    .Include(s => s.ActiveAssociation)
                    .ThenInclude(a => a!.PermissionSet);
            }
            CurrentSession = await query
                .SingleOrDefaultAsync(s => s.Id == sessionId && s.Voided == null);
            CurrentSessionUpdated?.Invoke(this, EventArgs.Empty);
            return CurrentSession;
        }

        public async Task VoidSessionAsync(UserSession? session = null) {
            session = session ?? CurrentSession;
            if(session is null) {
                return;
            }
            session.Voided = _timeService.GetNow();
            await _dbContext.SaveChangesAsync();
        }

    }
}
