using System.Collections.Immutable;

namespace IdleSchemes.WebAdmin.ViewModels {

    public interface IListViewModel {
        ImmutableList<int> LeftPages { get; }
        ImmutableList<int> RightPages { get; } 
        int TotalObjsCount { get; }
        int TotalPages { get; }
        int CurrentPage { get; }
        bool ShowFirst { get; }
        bool ShowLast { get; }
        Task GoToPageAsync(int pageNumber);
    }

    public abstract class ListViewModel<TObj> : ViewModelBase, IListViewModel {

        public ImmutableList<TObj> PageList { get; private set; } = ImmutableList<TObj>.Empty;
        public ImmutableList<int> LeftPages { get; private set; } = ImmutableList<int>.Empty;
        public ImmutableList<int> RightPages { get; private set; } = ImmutableList<int>.Empty;
        public int TotalObjsCount { get; private set; }
        public int TotalPages { get; private set; }
        public int CurrentPage { get; private set; }
        public bool ShowFirst { get; private set; }
        public bool ShowLast { get; private set; }
        public virtual int ObjsPerPage { get; } = 50;

        protected override async Task InitializeAsync() {
            await ResetAsync();
        }

        public async Task ResetAsync() {
            TotalObjsCount = await CountAllAsync();
            TotalPages = TotalObjsCount / ObjsPerPage;
            if (TotalObjsCount % ObjsPerPage != 0) {
                TotalPages++;
            }
            await GoToPageAsync(1);
        }

        public async Task GoToPageAsync(int pageNumber) {
            CurrentPage = pageNumber;
            var offset = pageNumber - 1;
            var results = await FetchPageAsync(offset * ObjsPerPage);
            PageList = results
                .Take(ObjsPerPage)
                .ToImmutableList();
            LeftPages = ImmutableList<int>.Empty;
            for (int i = CurrentPage - 1; i >= 1; i--) {
                LeftPages = LeftPages.Prepend(i).ToImmutableList();
            }
            RightPages = ImmutableList<int>.Empty;
            for (int i = CurrentPage + 1; i <= TotalPages; i++) {
                RightPages = RightPages.Append(i).ToImmutableList();
            }
            ShowFirst = LeftPages.Any() && !LeftPages.Contains(1);
            ShowLast = RightPages.Any() && !RightPages.Contains(TotalPages);
            Refresh();
        }

        protected abstract Task<int> CountAllAsync();
        protected abstract Task<List<TObj>> FetchPageAsync(int skip);

    }
}
