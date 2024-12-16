using BookStore.Command;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Collections.ObjectModel;

namespace BookStore.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private Store _selectedStore;

        private string _bookFilter;

        public ObservableCollection<Store> Stores { get; set; }

        public ObservableCollection<StockBalance> AllBooks { get; set; }

        public ObservableCollection<StockBalance> BooksInStock { get; set; }

        public ObservableCollection<StockBalance> BooksOutOfStock { get; set; }

        public DelegateCommand SaveStockBalanceCommand { get; }

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

        public string BookFilter
        {
            get { return _bookFilter; }
            set
            {
                if (_bookFilter != value)
                {
                    _bookFilter = value;
                    RaisePropertyChanged();
                    FilterBooks();
                }
            }
        }

        public MainWindowViewModel()
        {
            SaveStockBalanceCommand = new DelegateCommand(o => SaveStockBalance());
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

            FilterBooks();
            RaisePropertyChanged(nameof(AllBooks));
        }


        private void FilterBooks()
        {
            if (AllBooks == null)
                return;

            var filteredBooks = AllBooks.Where(b =>
            string.IsNullOrEmpty(BookFilter) ||
            b.Isbn13Navigation.Title.Contains(BookFilter, StringComparison.OrdinalIgnoreCase)
            ).ToList();

            BooksInStock = new ObservableCollection<StockBalance>(filteredBooks.Where(b => b.Amount > 0));
            BooksOutOfStock = new ObservableCollection<StockBalance>(filteredBooks.Where(b => b.Amount == 0));

            RaisePropertyChanged(nameof(BooksInStock));
            RaisePropertyChanged(nameof(BooksOutOfStock));
        }


        private void SaveStockBalance()
        {
            using var db = new BookStoreContext();

            var stockBalances = db.StockBalances
                                  .Where(sb => sb.StoreId == SelectedStore.Id)
                                  .ToList();


            var allBooks = new List<StockBalance>(BooksInStock.Concat(BooksOutOfStock));

            foreach (var stockBalance in allBooks)
            {
                var dbStockBalance = stockBalances.FirstOrDefault(sb => sb.Isbn13 == stockBalance.Isbn13);

                if (dbStockBalance != null)
                {
                    dbStockBalance.Amount = stockBalance.Amount;
                }
                else
                {
                    db.StockBalances.Add(new StockBalance
                    {
                        StoreId = SelectedStore.Id,
                        Isbn13 = stockBalance.Isbn13,
                        Amount = stockBalance.Amount
                    });
                }
            }

            db.SaveChanges();
            LoadBooks();
        }


    }
}
