namespace IdleSchemes.Core.Services {
    public interface INavigationManager {
        void NavigateTo(string path, bool forceLoad = false);
    }
}
