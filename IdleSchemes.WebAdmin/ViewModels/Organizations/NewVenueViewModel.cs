using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;
using IdleSchemes.Data;
using IdleSchemes.Data.Models.Organizations;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class NewVenueViewModel : ViewModelBase {

        private readonly IdleDbContext _dbContext;
        private readonly IdService _idService;
        private readonly INavigationManager _navigationManager;

        public NewVenueViewModel(IdleDbContext dbContext, IdService idService, INavigationManager navigationManager) {
            _dbContext = dbContext;
            _idService = idService;
            _navigationManager = navigationManager;
        }

        public string? ErrorMessage { get; private set; } = null;
        public Venue? Venue { get; private set; }

        protected override Task InitializeAsync() {
            Venue = new Venue {
                Id = _idService.GenerateId(),
                Organization = CurrentOrganization!
            };
            _dbContext.Venues.Add(Venue);
            return base.InitializeAsync();
        }

        public async Task SubmitAsync() {
            try {
                await _dbContext.SaveChangesAsync();
                _navigationManager.NavigateTo("/org/venues");
            } catch (Exception ex) {
                ErrorMessage = ex.Message;
            }
        }
    }
}
