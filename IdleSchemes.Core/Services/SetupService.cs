using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;

namespace IdleSchemes.Core.Services {
    public class SetupService {

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly TimeService _timeService;
        private readonly UserService _userService;

        public SetupService(IdleDbContext dbContext, IdService idService, TimeService timeService, UserService userService) {
            _dbContext = dbContext;
            _idService = idService;
            _timeService = timeService;
            _userService = userService;
        }

        public async Task SetupAsync() {
            if(!await _dbContext.Database.EnsureCreatedAsync()) {
                return;
            }

            var now = _timeService.GetNow();

            var admin = await _userService.GetOrCreateUserAsync(email: "admin@idleschemes.com", name: "Administrator");
            admin.IsAdministrator = true;
            _userService.SetPassword(admin, "admin");

            _dbContext.PermissionSets.Add(new PermissionSet {
                Id = _idService.GenerateId(),
                Organization = null,
                Name = "Allow All",
                CanScheduleEvents = OwnAny.Any,
                CanViewEvents = OwnAny.Any,
                CanPublishEvents = OwnAny.Any,
                CanCancelEvents = OwnAny.Any,
                CanManagePatrons = true,
                CanManageAssociates = true,
                CanViewFinances = OwnAny.Any,
                CanManageOrganizationDetails = true
            });

            _dbContext.PermissionSets.Add(new PermissionSet {
                Id = _idService.GenerateId(),
                Organization = null,
                Name = "Disallow All",
                CanScheduleEvents = OwnAny.None,
                CanViewEvents = OwnAny.None,
                CanPublishEvents = OwnAny.None,
                CanCancelEvents = OwnAny.None,
                CanManagePatrons = false,
                CanManageAssociates = false,
                CanViewFinances = OwnAny.None,
                CanManageOrganizationDetails = false
            });

            await _dbContext.SaveChangesAsync();
        }

    }
}
