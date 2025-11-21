using IdleSchemes.Core.Helpers;
using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models;
using IdleSchemes.Data.Models.Events;
using IdleSchemes.Data.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace IdleSchemes.Tests.Helpers {
    public class TestSetupService {

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly TimeService _timeService;
        private readonly UserService _userServce;
        private readonly SetupService _setupService;
        private readonly TestEventHelper _testEventHelper;

        public TestSetupService(IdleDbContext dbContext, IdService idService, TimeService timeService, UserService userService,
            SetupService setupService, TestEventHelper testEventHelper) {
            _dbContext = dbContext;
            _idService = idService;
            _timeService = timeService;
            _userServce = userService;
            _setupService = setupService;
            _testEventHelper = testEventHelper;
        }

        public async Task DestroyAsync() {
            await _dbContext.Database.EnsureDeletedAsync();
            await _dbContext.SaveChangesAsync();
        }

        public async Task SetupAsync() {
            await _setupService.SetupAsync();
            var now = _timeService.GetNow();

            var timeZones = TimeZoneInfo.GetSystemTimeZones();

            #region Users
            var ethan = await _userServce.GetOrCreateUserAsync(email: "ethan@idleschemes.com", name: "Ethan");
            _userServce.SetPassword(ethan, "ethan");
            var madeline = await _userServce.GetOrCreateUserAsync(email: "madeline@idleschemes.com", name: "Madeline");
            var vanessa = await _userServce.GetOrCreateUserAsync(email: "vanessa@idleschemes.com", name: "Vanessa");
            var eli = await _userServce.GetOrCreateUserAsync(email: "eli@idleschemes.com", name: "Eli");
            #endregion

            #region Regions
            var atx = _dbContext.Regions.Add(new Region {
                Id = "atx",
                Name = "Austin, Texas",
                IsPublic = true,
                TimeZone = TimeHelper.CentralTimeZone.Id
            }).Entity;
            var den = _dbContext.Regions.Add(new Region {
                Id = "den",
                Name = "Denver, Colorado",
                IsPublic = true,
                TimeZone = TimeHelper.MountainTimeZone.Id
            }).Entity;
            var abq = _dbContext.Regions.Add(new Region {
                Id = "abq",
                Name = "Abuquerque, New Mexico",
                IsPublic = false,
                TimeZone = TimeHelper.MountainTimeZone.Id
            });
            #endregion

            #region Organizations
            var allPermissions = await _dbContext.PermissionSets
                .FirstAsync(ps => ps.Name == "Allow All" && ps.Organization == null);

            var brushAndNeedle = _dbContext.Organizations.Add(new Organization {
                Id = _idService.GenerateId(),
                Name = "Brush and Needle",
                Description = "A fun crafting studio for all ages!",
                TimeZone = den.TimeZone,
                Created = now,
                Region = den,
                RegionApproved = true,
                RegionScore = 0,
            }).Entity;
            var ethanBrushAndNeedle = _dbContext.Associates.Add(new Associate {
                Id = _idService.GenerateId(),
                User = ethan,
                Organization = brushAndNeedle,
                PermissionSet = allPermissions,
                Since = now,
                IsPublic = true,
                PublicName = "Ethan (B&N)",
                PublicBio = "A cool dude!"
            }).Entity;
            var madelineBrushAndNeedle = _dbContext.Associates.Add(new Associate {
                Id = _idService.GenerateId(),
                User = madeline,
                Organization = brushAndNeedle,
                PermissionSet = allPermissions,
                Since = now,
                IsPublic = true,
                PublicName = "Madeline (B&N)",
                PublicBio = "A cool gal!"
            }).Entity;
            var acr = _dbContext.Organizations.Add(new Organization {
                Id = _idService.GenerateId(),
                Name = "Austin Creative Reuse",
                Description = "Tons of used art supplies and classes!",
                TimeZone = atx.TimeZone,
                Created = now,
                Region = atx,
                RegionApproved = true,
                RegionScore = 0,
            }).Entity;
            var craft = _dbContext.Organizations.Add(new Organization {
                Id = _idService.GenerateId(),
                Name = "Craft",
                Description = "A fun crafting studio for adults only!",
                TimeZone = atx.TimeZone,
                Created = now,
                Region = atx,
                RegionApproved = false,
                RegionScore = 0,
            }).Entity;
            var ronkita = _dbContext.Organizations.Add(new Organization {
                Id = _idService.GenerateId(),
                Name = "Ronkita Design",
                Description = "Personalized and group sewing lessons!",
                TimeZone = TimeHelper.CentralTimeZone.ToSerializedString(),
                Created = now,
            }).Entity;
            #endregion

            #region Events
            var ev1 = _testEventHelper.CreateEventInstance("B&N Event 1", brushAndNeedle, [ethanBrushAndNeedle],
                [(now.Date.AddDays(1).AddHours(17), now.Date.AddDays(1).AddHours(19))],
                [new TicketClassCreationOptions { Name = "General", Available = 10 }]);
            ev1.ListInRegion = true;
            ev1.Published = now;

            var ev2 = _testEventHelper.CreateEventInstance("B&N Event 2", brushAndNeedle, [madelineBrushAndNeedle],
                [(now.Date.AddDays(2).AddHours(17), now.Date.AddDays(2).AddHours(19))],
                [new TicketClassCreationOptions { Name = "Class 1", Available = 5 }, new TicketClassCreationOptions { Name = "Class 2", Available = 5 }]);
            ev2.ListInRegion = true;
            ev2.Published = now;

            var ev3 = _testEventHelper.CreateEventInstance("B&N Event 3", brushAndNeedle, [ethanBrushAndNeedle],
                [(now.Date.AddDays(3).AddHours(17), now.Date.AddDays(3).AddHours(19))],
                [new TicketClassCreationOptions { Name = "General", Available = 10 }]);
            ev3.Published = now;

            var ev4 = _testEventHelper.CreateEventInstance("B&N Event 4", brushAndNeedle, [ethanBrushAndNeedle],
                [(now.Date.AddDays(4).AddHours(17), now.Date.AddDays(4).AddHours(19))],
                [new TicketClassCreationOptions { Name = "General", Available = 10 }]);

            #endregion

            await _dbContext.SaveChangesAsync();
        }

    }
}
