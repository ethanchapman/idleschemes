using IdleSchemes.Core.Services;

namespace IdleSchemes.Tests.Helpers {
    public class MockNavigationManager : INavigationManager {
        public MockNavigationManager() { }
        public string? NavigatedToPath { get; private set; } = null;
        public void NavigateTo(string path, bool forceLoad = false) {
            NavigatedToPath = path;
        }
    }
}
