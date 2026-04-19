namespace LibraryAppInteractive.Business_Logic;

public class LibraryAsset
{
    private Book _book;
    private int _libID;
    private AssetStatus _status;
    private LoanPeriod _loanPeriod;

    public int LibId
    {
        get => _libID;
        set => _libID = value;
    }
    public AssetStatus Status
    {
        get => _status;
        set => _status = value;
    }

    public LoanPeriod Loan
    {
        get => _loanPeriod;
        set => _loanPeriod = value;
    }
    public bool IsAvailable()
    {
        return Status == AssetStatus.Available;
    }

    public LibraryAsset(int libID, Book book)
    {
        _libID = libID;
        _book = book;
        _status = AssetStatus.Available;
    }
}

