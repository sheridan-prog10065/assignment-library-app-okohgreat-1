namespace LibraryAppInteractive.Business_Logic;

public class Book
{
    private string _bookName;
    private string _bookISBN;
    private List<string> _bookAuthorList;
    private List<LibraryAsset> _libAssetList;
    private BookType _bookType;

    public string Name
    {
        get => _bookName;
        set => _bookName = value;
    }

    public string ISBN
    {
        get => _bookISBN;
        set => _bookISBN = value;
    }

    public List<string> Authors
    {
        get => _bookAuthorList;
        set => _bookAuthorList = value;
    }

    public List<LibraryAsset> Assets
    {
        get => _libAssetList;
        set => _libAssetList = value;
    }

    public BookType BookType
    {
        get => _bookType;
        set => _bookType = value;
    }

    public Book(string bookName, string bookISBN, BookType bookType = BookType.Paper)
    {
        _bookName = bookName;
        _bookISBN = bookISBN;
        _bookType = bookType;
        _bookAuthorList = new List<string>();
        _libAssetList = new List<LibraryAsset>();
    }

    /// <summary>
    /// Checks the availability of the book by checking if there are any library assets 
    /// for this book that are available to borrow.
    /// Returns:
    ///     (true, null) - an asset is available and could be loaned
    ///     (false, DateTime) - all assets are loaned or reserved. Returns the earliest available date.
    /// </summary>
    public (bool, DateTime?) CheckAvailability()
    {
        LibraryAsset nextAvailAsset = FindNextAvailableAsset();

        if (nextAvailAsset?.Status == AssetStatus.Available)
        {
            return (true, null);
        }
        else
        {
            return (false, nextAvailAsset?.DueDate);
        }
    }

    /// <summary>
    /// Attempts to find an available asset and loans it to the user.
    /// If no book asset is available, throws an exception.
    /// </summary>
    public virtual LibraryAsset BorrowBook()
    {
        LibraryAsset libraryAsset = FindNextAvailableAsset();

        if (libraryAsset?.Status != AssetStatus.Available)
        {
            throw new InvalidOperationException("The requested book is not available. You can check for availability first and reserve it.");
        }

        libraryAsset.BorrowedOn = DateTime.Now;
        libraryAsset.Status = AssetStatus.Loaned;

        // Set default due date (14 days). Derived classes can override this.
        libraryAsset.DueDate = DateTime.Now.AddDays(14);

        return libraryAsset;
    }

    /// <summary>
    /// Returns a borrowed asset back to the library using the ID of the library asset.
    /// </summary>
    public virtual (TimeSpan, int, decimal) ReturnBook(int libID)
    {
        LibraryAsset libraryAsset = FindLibraryAsset(libID);
        libraryAsset.ReturnedOn = DateTime.Now;

        TimeSpan loanDuration = libraryAsset.GetLoanDuration();
        TimeSpan latePeriod = libraryAsset.GetLatePeriod();

        libraryAsset.Status = AssetStatus.Available;

        // Base method does not calculate late penalties. Derived classes must perform this.
        return (loanDuration, latePeriod.Days, 0.0m);
    }

    /// <summary>
    /// Finds and reserves the earliest available asset for this book.
    /// </summary>
    public LibraryAsset ReserveBook()
    {
        LibraryAsset nextAvailAsset = FindNextAvailableAsset();
        nextAvailAsset.Status = AssetStatus.Reserved;
        return nextAvailAsset;
    }

    /// <summary>
    /// Finds the library asset with the given ID.
    /// Throws an exception if no asset is found.
    /// </summary>
    private LibraryAsset FindLibraryAsset(int libID)
    {
        foreach (LibraryAsset asset in _libAssetList)
        {
            if (asset.LibId == libID)
            {
                return asset;
            }
        }

        throw new InvalidOperationException($"An asset with ID = {libID} was not found for book {Name}");
    }

    /// <summary>
    /// Finds the next available asset for this book.
    /// If an asset is available immediately, it returns that one.
    /// Otherwise, returns the asset with the earliest due date.
    /// </summary>
    private LibraryAsset FindNextAvailableAsset()
    {
        LibraryAsset nextAvailAsset = null;

        foreach (LibraryAsset asset in _libAssetList)
        {
            // Check if asset is available immediately
            if (asset.Status == AssetStatus.Available)
            {
                return asset;
            }

            // Find asset with the earliest due date
            if (nextAvailAsset == null || 
                (nextAvailAsset.DueDate > asset.DueDate && nextAvailAsset.Status != AssetStatus.Reserved))
            {
                nextAvailAsset = asset;
            }
        }

        return nextAvailAsset;
    }
}