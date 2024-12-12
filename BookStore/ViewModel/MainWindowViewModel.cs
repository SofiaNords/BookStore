﻿using BookStore.Command;
using BookStore.Dialogs;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;
using System.Collections.ObjectModel;

namespace BookStore.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private Store _selectedStore;

        public ObservableCollection<Store> Stores { get; set; }

        public ObservableCollection<StockBalance> BooksInStock { get; set; }

        public ObservableCollection<Book> BooksOutOfStock { get; set; }

        public DelegateCommand OpenIncreaseDialogCommand { get; }

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
            BooksInStock = new ObservableCollection<StockBalance>();
            BooksOutOfStock = new ObservableCollection<Book>();
            OpenIncreaseDialogCommand = new DelegateCommand(OpenIncreaseStockDialog);

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
            if (SelectedStore != null)
            {
                using (var _bookStoreContext = new BookStoreContext())
                {
                    var allBooks = _bookStoreContext.Books
                        .Include(b => b.Author)
                        .Include(b => b.Genre)
                        .ToList();

                    var booksInStock = _bookStoreContext.StockBalances
                        .Include(sb => sb.Isbn13Navigation.Author)
                        .Include(sb => sb.Isbn13Navigation.Genre)
                        .Where(sb => sb.StoreId == SelectedStore.Id && sb.Amount > 0)
                        .ToList();

                    var booksOutOfStock = allBooks
                        .Where(b => !_bookStoreContext.StockBalances
                            .Any(sb => sb.StoreId == SelectedStore.Id && sb.Isbn13 == b.Isbn13 && sb.Amount > 0))
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

        private void OpenIncreaseStockDialog(object parameter)
        {
            var dialog = new IncreaseStockDialog();

            dialog.DataContext = this;

            dialog.ShowDialog();
        }
    }
}
