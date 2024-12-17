using BookStore.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            AllAuthors = new ObservableCollection<Author>(books.Select(b => b.Author).Distinct().ToList());
            AllGenres = new ObservableCollection<Genre>(books.Select(b => b.Genre).Distinct().ToList());
            AllPublishers = new ObservableCollection<Publisher>(books.Select(b => b.Publisher).Distinct().ToList());
        }

        public void SaveNewBook(object parameter)
        {
            using var db = new BookStoreContext();

            var author = db.Authors
                   .FirstOrDefault(a => a.FirstName == NewBook.Author.FirstName && a.LastName == NewBook.Author.LastName);
            var genre = db.Genres.FirstOrDefault(g => g.Name == NewBook.Genre.Name);
            var publisher = db.Publishers.FirstOrDefault(p => p.Name == NewBook.Publisher.Name);


            if (author == null || genre == null || publisher == null)
            {
                MessageBox.Show("Plese make sure all fields are correctly filled.");
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
