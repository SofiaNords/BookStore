using BookStore.Command;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;

namespace BookStore.ViewModel
{
    internal class AddNewBookViewModel : ViewModelBase
    {
        public ObservableCollection<Book> AllBooks { get; set; }

        public ObservableCollection<Author> AllAuthors { get; set; }

        public ObservableCollection<Genre> AllGenres { get; set; }

        public ObservableCollection<Publisher> AllPublishers { get; set; }

        public Book NewBook { get; set; }

        public DelegateCommand SaveCommand { get; set; }

        public AddNewBookViewModel()
        {
            NewBook = new Book();
            SaveCommand = new DelegateCommand(SaveNewBook);
            LoadAllBooks();
        }


        public void LoadAllBooks()
        {
            using var db = new BookStoreContext();

            var books = db.Books
                          .Include(b => b.Author)
                          .Include(b => b.Genre)
                          .Include(b => b.Publisher)
                          .ToList();

            AllBooks = new ObservableCollection<Book>(books);

            AllAuthors = new ObservableCollection<Author>(db.Authors.ToList());
            AllGenres = new ObservableCollection<Genre>(db.Genres.ToList());
            AllPublishers = new ObservableCollection<Publisher>(db.Publishers.ToList());
        }


        public void SaveNewBook(object parameter)
        {
            using var db = new BookStoreContext();

            if (string.IsNullOrEmpty(NewBook.Title))
            {
                MessageBox.Show("Please enter a valid title.");
                return;
            }

            if (NewBook.Isbn13 <= 0)
            {
                MessageBox.Show("Please enter a valid ISBN13.");
                return;
            }

            var isbnString = NewBook.Isbn13.ToString();
            if (isbnString.Length != 13)
            {
                MessageBox.Show("Please enter a valid ISBN13 consisting of exactly 13 digits.");
                return;
            }

            var existingBook = db.Books.FirstOrDefault(b => b.Isbn13 == NewBook.Isbn13);
            if (existingBook != null)
            {
                MessageBox.Show("This ISBN already exists in the database.");
                return;
            }

            if (string.IsNullOrEmpty(NewBook.Language))
            {
                MessageBox.Show("Please enter a valid language.");
                return;
            }

            if (NewBook.Price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than 0.");
                return;
            }

            if (NewBook.YearOfPublication <= 0)
            {
                MessageBox.Show("Please enter a valid year of publication.");
                return;
            }

            if (NewBook.Author == null || string.IsNullOrEmpty(NewBook.Author.FirstName) || string.IsNullOrEmpty(NewBook.Author.LastName))
            {
                MessageBox.Show("Please select or enter a valid author.");
                return;
            }

            if (NewBook.Genre == null || string.IsNullOrEmpty(NewBook.Genre.Name))
            {
                MessageBox.Show("Please select a valid genre.");
                return;
            }

            if (NewBook.Publisher == null || string.IsNullOrEmpty(NewBook.Publisher.Name))
            {
                MessageBox.Show("Please select a valid publisher.");
                return;
            }

            var author = db.Authors
                .FirstOrDefault(a => a.FirstName == NewBook.Author.FirstName && a.LastName == NewBook.Author.LastName);
            if (author == null)
            {
                MessageBox.Show("The author could not be found. Please ensure the author exists in the database.");
                return;
            }

            var genre = db.Genres.FirstOrDefault(g => g.Name == NewBook.Genre.Name);
            if (genre == null)
            {
                MessageBox.Show("The genre could not be found. Please ensure the genre exists in the database.");
                return;
            }

            var publisher = db.Publishers.FirstOrDefault(p => p.Name == NewBook.Publisher.Name);
            if (publisher == null)
            {
                MessageBox.Show("The publisher could not be found. Please ensure the publisher exists in the database.");
                return;
            }

            NewBook.Author = author;
            NewBook.Genre = genre;
            NewBook.Publisher = publisher;

            db.Books.Add(NewBook);
            db.SaveChanges();

            AllBooks.Add(NewBook);
            NewBook = new Book();

            MessageBox.Show("The book is saved!");

            LoadAllBooks();
        }
    }
}
