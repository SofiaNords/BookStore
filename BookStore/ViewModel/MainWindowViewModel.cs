using System.Collections.ObjectModel;

namespace BookStore.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public ObservableCollection<Store> Stores { get; set; }

        public MainWindowViewModel()
        {
            Stores = new ObservableCollection<Store>();

            LoadStores();

        }

        private void LoadStores()
        {
            using (var _bookStoreContext = new BookStoreContext())
            {
                var stores = _bookStoreContext.Stores.ToList();

                foreach (var store in stores)
                {
                    Stores.Add(store);
                }
            }
        }

    }
}
