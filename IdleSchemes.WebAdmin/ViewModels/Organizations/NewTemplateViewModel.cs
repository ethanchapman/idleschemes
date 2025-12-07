using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class NewTemplateViewModel : ViewModelBase {

        private readonly EventFactoryService _eventFactoryService;
        private readonly INavigationManager _navigationManager;

        public NewTemplateViewModel(EventFactoryService eventFactoryService, INavigationManager navigationManager) {
            _eventFactoryService = eventFactoryService;
            _navigationManager = navigationManager;
        }

        public string? ErrorMessage { get; private set; } = null;
        public EventTemplateCreationOptions Form { get; } = new EventTemplateCreationOptions();
        public bool HasIndividualTicketLimit { get; set; }

        protected override Task InitializeAsync() {
            Form.OrganizationId = CurrentSession!.ActiveAssociation!.Organization.Id;
            return base.InitializeAsync();
        }

        public async Task SubmitAsync() {
            try {
                if (!HasIndividualTicketLimit) {
                    Form.IndividualTicketLimit = null;
                }
                var template = await _eventFactoryService.CreateEventTemplateAsync(Form, true);
                _navigationManager.NavigateTo("/org/templates");
            } catch (Exception ex) {
                ErrorMessage = ex.Message;
            }
        }
    }
}
