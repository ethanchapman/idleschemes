using IdleSchemes.WebAdmin.ViewModels;
using IdleSchemes.Core.Services;
using Microsoft.AspNetCore.Components;

namespace IdleSchemes.WebAdmin.Components {
    public class BaseAdminComponent<TViewModel> : ComponentBase where TViewModel : ViewModelBase {

        [Inject]
        protected UserSessionService UserSessionService { get; init; } = null!;
        [Inject]
        protected IHttpContextAccessor IHttpContextAccessor { get; init; } = null!;
        [Inject]
        protected NavigationManager NavigationManager { get; init; } = null!;
        [Inject]
        protected TViewModel ViewModel { get; init; } = null!;

        protected override async Task OnInitializedAsync() {
            var context = IHttpContextAccessor.HttpContext!;
            await UserSessionService.TryResumeSessionAsync(context, includeActiveAssociate: true);
            if (ViewModel.RequireUser && UserSessionService.CurrentSession is null) {
                NavigationManager.NavigateTo("/login");
            }
            await ViewModel.InitializeAsync(UserSessionService.CurrentSession, StateHasChanged);
        }

    }
}
