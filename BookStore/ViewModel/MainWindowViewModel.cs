using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BookStore.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private Store _selectedStore;

        public ObservableCollection<Store> Stores { get; set; }

        public ObservableCollection<Book> BooksInStock { get; set; }

        public ObservableCollection<Book> BooksOutOfStock { get; set; }

        public Store SelectedStore
        {
            get { return _selectedStore; }
            set 
            { 
                if (_selectedStore != value)
                {
                    _selectedStore = value;
                    RaisePropertyChanged(nameof(SelectedStore));
                    LoadBooks();
                }    
            }
        }
     
        public MainWindowViewModel()
        {
            Stores = new ObservableCollection<Store>();
            BooksInStock = new ObservableCollection<Book>();
            BooksOutOfStock = new ObservableCollection<Book>();

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
            if (SelectedStore != null)
            {
                using (var _bookStoreContext = new BookStoreContext())
                {
                    var booksInStock = _bookStoreContext.Books
                        .Include(b => b.StockBalances)
                        .Include(b => b.Author)
                        .Include(b => b.Genre)
                        .Where(b => b.StockBalances.Any(sb => sb.StoreId == SelectedStore.Id && sb.Amount > 0))
                        .ToList();


                    var booksOutOfStock = _bookStoreContext.Books
                        .Include(b => b.StockBalances)
                        .Include(b => b.Author)
                        .Include(b => b.Genre)
                        .Where(b => !b.StockBalances.Any(sb => sb.StoreId == SelectedStore.Id && sb.Amount > 0))
                        .ToList();

                    BooksInStock.Clear();
                    BooksOutOfStock.Clear();

                    foreach (var book in booksInStock)
                    {
                        BooksInStock.Add(book);
                    }

                    foreach (var book in booksOutOfStock)
                    {
                        BooksOutOfStock.Add(book);
                    }
                }
            }
        }
    }
}
