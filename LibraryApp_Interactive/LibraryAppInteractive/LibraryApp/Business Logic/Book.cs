namespace LibraryAppInteractive.Business_Logic;

public class Book
{
    private string _bookName;
    private string _bookISBN;
    private List<string> _bookAuthorList;
    private List<LibraryAsset> _libAssetList;

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

    public Book(string bookName, string bookISBN)
    {
        
    }

    public (bool, LibraryAsset) CheckAvailability()
    {
        
    }
    
    public LibraryAsset BorrowBook()
    {
        
    }

    public (TimeSpan, int, decimal) ReturnBook(int libID)
    {
        
    }
    
    public LibraryAsset ReserveBook()
    {
        
    }
    
    private LibraryAsset findLibraryAsset(int libID)
    {
        
    }
    
    private LibraryAsset findNextavailableAsset()
    {
        
    }
}