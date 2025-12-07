using IdleSchemes.Core.Services;
using Microsoft.AspNetCore.Components;

namespace IdleSchemes.WebAdmin.Services {
    public class NavigationManagerImpl : INavigationManager {

        private readonly NavigationManager _navigationManager;

        public NavigationManagerImpl(NavigationManager navigationManager) {
            _navigationManager = navigationManager;
        }

        public void NavigateTo(string path, bool forceLoad = false) {
            _navigationManager.NavigateTo(path, forceLoad);
        }

    }
}
