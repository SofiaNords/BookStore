using System.Collections.ObjectModel;

namespace BookStore.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private Store _selectedStore;

        public ObservableCollection<Store> Stores { get; set; }

        public Store SelectedStore
        {
            get { return _selectedStore; }
            set 
            { 
                if (_selectedStore != value)
                {
                    _selectedStore = value;
                    RaisePropertyChanged(nameof(SelectedStore));
                }    
            }
        }

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

        private void LoadBooks()
        {
            
        }

    }
}
