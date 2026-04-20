using LibraryAppInteractive.Business_Logic;
using System.Collections.ObjectModel;

namespace LibraryAppInteractive;

/// <summary>
/// Defines the Library class used to manage the library books and assets.
///
/// NOTE: A single object/instance of this class (called a "singleton") is created and shared automatically
/// with the two pages in the application through the process of Dependency Injection handled and configured
/// in MauiProgram class.  
/// </summary>
public class Library
{
    private List<Book> _bookList;
    private int _libIDGeneratorSeed;
    private const int DEFAULT_LIBID_START = 100;

    public Library()
    {
        _bookList = new List<Book>();
        _libIDGeneratorSeed = DEFAULT_LIBID_START;
        CreateDefaultBooks();
    }

    /// <summary>
    /// Creates default books with library assets to start
    /// </summary>
    private void CreateDefaultBooks()
    {
        // Create a paper book
        PaperBook demoPaperBook = new PaperBook("Lord of the Rings", "978-0261102385");
        demoPaperBook.Authors.Add("J.R.R. Tolkien");

        // Add five library assets for this book
        for (int i = 0; i < 5; i++)
        {
            LibraryAsset demoBookAsset = new LibraryAsset(DetermineLibID(), demoPaperBook);
            demoBookAsset.Status = AssetStatus.Available;
            demoPaperBook.Assets.Add(demoBookAsset);
        }

        _bookList.Add(demoPaperBook);

        // Create a digital book
        DigitalBook demoDigitalBook = new DigitalBook("Harry Potter", "978-1408898659");
        demoDigitalBook.Authors.Add("J.K. Rowling");

        // Add five library assets for this book
        for (int i = 0; i < 5; i++)
        {
            LibraryAsset demoBookAsset = new LibraryAsset(DetermineLibID(), demoDigitalBook);
            demoBookAsset.Status = AssetStatus.Available;
            demoDigitalBook.Assets.Add(demoBookAsset);
        }

        _bookList.Add(demoDigitalBook);
    }

    /// <summary>
    /// random unique library ID generator for assets
    /// </summary>
    private int DetermineLibID()
    {
        int libID = _libIDGeneratorSeed;
        _libIDGeneratorSeed++;
        return libID;
    }

    /// <summary>
    /// new book registration method that creates a new book and adds it to the library. It also creates the specified number of library assets for the book.
    /// </summary>
    public Book RegisterBook(string bookName, string bookISBN, string[] authors, BookType bookType, int nCopies)
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(bookName))
            throw new ArgumentException("Book name cannot be empty.");

        if (string.IsNullOrWhiteSpace(bookISBN))
            throw new ArgumentException("Book ISBN cannot be empty.");

        if (authors == null || authors.Length == 0)
            throw new ArgumentException("At least one author must be provided.");

        if (nCopies <= 0)
            throw new ArgumentException("Number of copies must be greater than 0.");

        // Check if book already exists
        Book existingBook = FindBookByISBN(bookISBN);
        if (existingBook != null)
        {
            throw new InvalidOperationException($"A book with ISBN {bookISBN} already exists in the library.");
        }

        // Create book based on type
        Book newBook;
        switch (bookType)
        {
            case BookType.Paper:
                newBook = new PaperBook(bookName, bookISBN);
                break;
            case BookType.Digital:
            case BookType.Audio:
                newBook = new DigitalBook(bookName, bookISBN);
                break;
            default:
                newBook = new Book(bookName, bookISBN, BookType.Paper);
                break;
        }

        // Add authors
        foreach (string author in authors)
        {
            newBook.Authors.Add(author);
        }

        // Create library assets for each copy
        for (int i = 0; i < nCopies; i++)
        {
            int assetID = DetermineLibID();
            LibraryAsset asset = new LibraryAsset(assetID, newBook)
            {
                Status = AssetStatus.Available
            };
            newBook.Assets.Add(asset);
        }

        // Add book to library
        _bookList.Add(newBook);
        return newBook;
    }

    /// <summary>
    /// Finds a book by name (case-insensitive).
    /// </summary>
    public Book FindBookByName(string bookName)
    {
        if (string.IsNullOrWhiteSpace(bookName))
            return null;

        return _bookList.FirstOrDefault(book => 
            book.Name.Equals(bookName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Finds a book by ISBN (case-insensitive).
    /// </summary>
    public Book FindBookByISBN(string bookISBN)
    {
        if (string.IsNullOrWhiteSpace(bookISBN))
            return null;

        return _bookList.FirstOrDefault(book => 
            book.ISBN.Equals(bookISBN, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Get all books in the library.
    /// </summary>
    public List<Book> GetAllBooks()
    {
        return new List<Book>(_bookList);
    }

    /// <summary>
    /// Checks if a book is available for borrowing.
    /// </summary>
    public bool CheckAvailability(string bookISBN)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            return false;

        (bool isAvailable, _) = book.CheckAvailability();
        return isAvailable;
    }

    /// <summary>
    /// Borrows a book asset for a user.
    /// </summary>
    public LibraryAsset BorrowBook(string bookISBN)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            throw new InvalidOperationException($"Book with ISBN {bookISBN} not found.");

        return book.BorrowBook();
    }

    /// <summary>
    /// Returns a borrowed book asset.
    /// </summary>
    public (TimeSpan, int, decimal) ReturnBook(string bookISBN, int assetID)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            throw new InvalidOperationException($"Book with ISBN {bookISBN} not found.");

        return book.ReturnBook(assetID);
    }

    /// <summary>
    /// Gets all assets for a specific book.
    /// </summary>
    public List<LibraryAsset> GetBookAssets(string bookISBN)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            return new List<LibraryAsset>();

        return new List<LibraryAsset>(book.Assets);
    }

    /// <summary>
    /// Gets count of available copies for a book.
    /// </summary>
    public int GetAvailableCount(string bookISBN)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            return 0;

        return book.Assets.Count(a => a.Status == AssetStatus.Available);
    }

    /// <summary>
    /// Adds a new asset to an existing book.
    /// </summary>
    public LibraryAsset AddAssetToBook(string bookISBN, AssetStatus initialStatus = AssetStatus.Available)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            throw new InvalidOperationException($"Book with ISBN {bookISBN} not found.");

        int assetID = DetermineLibID();
        LibraryAsset newAsset = new LibraryAsset(assetID, book)
        {
            Status = initialStatus
        };

        book.Assets.Add(newAsset);
        return newAsset;
    }

    /// <summary>
    /// Reserves a book for a user.
    /// </summary>
    public LibraryAsset ReserveBook(string bookISBN)
    {
        Book book = FindBookByISBN(bookISBN);
        if (book == null)
            throw new InvalidOperationException($"Book with ISBN {bookISBN} not found.");

        return book.ReserveBook();
    }
}