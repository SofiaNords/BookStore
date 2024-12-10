using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace BookStore;

public partial class BookStoreContext : DbContext
{
    public BookStoreContext()
    {
    }

    public BookStoreContext(DbContextOptions<BookStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<StockBalance> StockBalances { get; set; }

    public virtual DbSet<Store> Stores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = new SqlConnectionStringBuilder()
        {
            ServerSPN = "localhost",
            InitialCatalog = "BookStore",
            IntegratedSecurity = true,
            TrustServerCertificate = true
        }.ToString();

        optionsBuilder
            .UseSqlServer(connectionString)
            .LogTo(
                message => MyLogger(message),
                new[] { DbLoggerCategory.Database.Command.Name },
                LogLevel.Information,
                DbContextLoggerOptions.Level | DbContextLoggerOptions.LocalTime
            )
            .EnableSensitiveDataLogging();
    }

    private void MyLogger(string message)
    {
        var lines = message.Split('\n');

        Console.WriteLine();

        for (int i = 0; i < lines.Length; i++)
        {
            if (i == 0) Console.ForegroundColor = ConsoleColor.Blue;
            if (i == 1) Console.ForegroundColor = ConsoleColor.DarkGray;
            if (i == 2) Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(lines[i]);
        }

        Console.ResetColor();
        Console.WriteLine();
    }

    public void PrintChangeTrackerDebugInfo(string? caption = null, bool autoDetectChanges = true)
    {
        if (caption is not null)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(caption);
            Console.ResetColor();
        }

        if (autoDetectChanges)
        {
            this.ChangeTracker.DetectChanges();
        }

        var debugInfo = this.ChangeTracker.DebugView.LongView;

        var lines = debugInfo.Split("\n");

        foreach (var line in lines)
        {
            var status = line.Trim('\r');
            if (status.EndsWith("Deleted"))
            {
                Console.Write(status[..^7]);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Deleted");
                Console.ResetColor();
            }
            else if (status.EndsWith("Modified"))
            {
                Console.Write(status[..^8]);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Modified");
                Console.ResetColor();
            }
            else if (status.EndsWith("Unchanged"))
            {
                Console.Write(status[..^9]);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Unchanged");
                Console.ResetColor();
            }
            else if (status.Contains("Modified Originally"))
            {
                var stringStart = status.Split("Modified Originally").First();
                var newValue = stringStart.Trim().Split(" ").Last();
                var oldValue = status.Split("Modified Originally").Last();
                var indexOfNewValue = stringStart.IndexOf(newValue);

                Console.Write(stringStart[..indexOfNewValue]);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(newValue);
                Console.ResetColor();
                Console.Write(" Modified Originally");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(oldValue);
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(status);
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Author__3214EC07E01F5F04");

            entity.ToTable("Author");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Isbn13).HasName("PK__Books__3BF79E03598A7B05");

            entity.Property(e => e.Isbn13)
                .ValueGeneratedNever()
                .HasColumnName("ISBN13");
            entity.Property(e => e.Language).HasMaxLength(50);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Author).WithMany(p => p.Books)
                .HasForeignKey(d => d.AuthorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_AuthorId");

            entity.HasOne(d => d.Genre).WithMany(p => p.Books)
                .HasForeignKey(d => d.GenreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_GenreId");

            entity.HasOne(d => d.Publisher).WithMany(p => p.Books)
                .HasForeignKey(d => d.PublisherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Books_PublisherId");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Genre__3214EC075ACF568C");

            entity.ToTable("Genre");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Publishe__3214EC07802F5417");

            entity.ToTable("Publisher");

            entity.HasIndex(e => e.Name, "UQ__Publishe__737584F6B2293490").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PostalAdress).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.StreetAdress).HasMaxLength(100);
        });

        modelBuilder.Entity<StockBalance>(entity =>
        {
            entity.HasKey(e => new { e.StoreId, e.Isbn13 });

            entity.ToTable("StockBalance");

            entity.Property(e => e.Isbn13).HasColumnName("ISBN13");
            entity.Property(e => e.Amount).HasDefaultValue(0);

            entity.HasOne(d => d.Isbn13Navigation).WithMany(p => p.StockBalances)
                .HasForeignKey(d => d.Isbn13)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBalance_ISBN13");

            entity.HasOne(d => d.Store).WithMany(p => p.StockBalances)
                .HasForeignKey(d => d.StoreId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StockBalance_StoreId");
        });

        modelBuilder.Entity<Store>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Stores__3214EC07E6BBC50A");

            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.PostalAdress).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(50);
            entity.Property(e => e.StreetAdress).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
