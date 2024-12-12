using BookStore.Command;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Collections.ObjectModel;

namespace BookStore.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private Store _selectedStore;

        public ObservableCollection<Store> Stores { get; set; }

        public ObservableCollection<StockBalance> AllBooks { get; set; }

        public ObservableCollection<StockBalance> BooksInStock { get; set; }

        public ObservableCollection<StockBalance> BooksOutOfStock { get; set; }

        public Store SelectedStore
        {
            get { return _selectedStore; }
            set 
            { 
                if (_selectedStore != value)
                {
                    _selectedStore = value;
                    RaisePropertyChanged();
                    LoadBooks();
                }    
            }
        }
     
        public MainWindowViewModel()
        {
            LoadStores();
        }

        private void LoadStores()
        {
            using var db = new BookStoreContext();

            Stores = new ObservableCollection<Store>(
                db.Stores.ToList()
            );
        }


        private void LoadBooks()
        {
            if (SelectedStore == null)
            {
                return;
            }

            using var db = new BookStoreContext();

            var allBooks = db.Books
                             .Include(b => b.Author)
                             .Include(b => b.Genre)
                             .ToList();

            var stockBalances = db.StockBalances
                                  .Where(sb => sb.StoreId == SelectedStore.Id)
                                  .ToList();

            var booksWithStock = allBooks.Select(book =>
            {
                var stockBalance = stockBalances.FirstOrDefault(sb => sb.Isbn13 == book.Isbn13);
                return new StockBalance
                {
                    StoreId = SelectedStore.Id,
                    Isbn13 = book.Isbn13,
                    Amount = stockBalance?.Amount ?? 0,
                    Isbn13Navigation = book
                };
            }).ToList();

            AllBooks = new ObservableCollection<StockBalance>(booksWithStock);
            BooksInStock = new ObservableCollection<StockBalance>(booksWithStock.Where(b => b.Amount > 0));
            BooksOutOfStock = new ObservableCollection<StockBalance>(booksWithStock.Where(b => b.Amount == 0));

            RaisePropertyChanged(nameof(AllBooks));
            RaisePropertyChanged(nameof(BooksInStock));
            RaisePropertyChanged(nameof(BooksOutOfStock));
        }

    }
}
