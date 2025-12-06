using IdleSchemes.Core.Models.Input;
using IdleSchemes.Core.Services;

namespace IdleSchemes.WebAdmin.ViewModels.Organizations {
    public class NewTemplateViewModel : ViewModelBase {

        private readonly EventFactoryService _eventFactoryService;

        public NewTemplateViewModel(EventFactoryService eventFactoryService) {
            _eventFactoryService = eventFactoryService;
        }

        public EventTemplateCreationOptions Form { get; } = new EventTemplateCreationOptions();
        public bool HasIndividualTicketLimit { get; set; }

        protected override Task InitializeAsync() {
            Form.OrganizationId = CurrentSession!.ActiveAssociation!.Organization.Id;
            return base.InitializeAsync();
        }

        public async Task SubmitAsync() {
            if(!HasIndividualTicketLimit) {
                Form.IndividualTicketLimit = null;
            }
            await _eventFactoryService.CreateEventTemplateAsync(Form);

        }
    }
}
