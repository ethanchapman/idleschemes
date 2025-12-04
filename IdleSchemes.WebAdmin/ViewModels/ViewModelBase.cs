using IdleSchemes.Core.Helpers;
using IdleSchemes.Data.Models;
using IdleSchemes.Data.Models.Organizations;

namespace IdleSchemes.WebAdmin.ViewModels {
    public abstract class ViewModelBase {

        protected Action Refresh { get; private set; } = () => { };

        public virtual bool RequireUser { get; } = true;
        public UserSession? CurrentSession { get; private set; }
        public Organization? CurrentOrganization => CurrentSession?.ActiveAssociation?.Organization;

        public async Task InitializeAsync(UserSession? currentSession, Action refresh) {
            CurrentSession = currentSession;
            Refresh = refresh;
            await InitializeAsync();
        }

        protected virtual Task InitializeAsync() {
            // Do Nothing
            return Task.CompletedTask;
        }

        public DateTime ConvertFromUtc(DateTime dateTime) {
            return TimeHelper.ConvertFromUtc(dateTime, TimeZoneId);
        }

        public DateTime ConvertToUtc(DateTime dateTime) {
            return TimeHelper.ConvertToUtc(dateTime, TimeZoneId);
        }

        public string? TimeZoneId => CurrentSession?.ActiveAssociation?.Organization.TimeZone ?? CurrentSession?.User?.TimeZone;

    }
}
