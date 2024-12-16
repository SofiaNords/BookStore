using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.ViewModel
{
    internal class AddNewBookViewModel : ViewModelBase
    {
        public ObservableCollection<Book> AllBooks { get; set; }

        public ObservableCollection<Author> AllAuthors { get; set; }

        public ObservableCollection<Genre> AllGenres { get; set; }

        public ObservableCollection<Publisher> AllPublishers { get; set; }

        public AddNewBookViewModel()
        {
            LoadAllBooks();
            LoadAllAuthors();
            LoadAllGenres();
            LoadAllPublishers();
        }

        public void LoadAllBooks()
        {
            using var db = new BookStoreContext();

            AllBooks = new ObservableCollection<Book>(
                db.Books
                    .Include(b => b.Author)
                    .Include(b => b.Genre)
                    .Include(b => b.Publisher)
                    .ToList()
                );
        }

        public void LoadAllAuthors()
        {
            using var db = new BookStoreContext();

            AllAuthors = new ObservableCollection<Author>(
                db.Authors.ToList()
                );
        }

        public void LoadAllGenres()
        {
            using var db = new BookStoreContext();

            AllGenres = new ObservableCollection<Genre>(
                db.Genres.ToList()
                );
        }

        public void LoadAllPublishers()
        {
            using var db = new BookStoreContext();

            AllPublishers = new ObservableCollection<Publisher>(
                db.Publishers.ToList()
                );
        }
    }
}
