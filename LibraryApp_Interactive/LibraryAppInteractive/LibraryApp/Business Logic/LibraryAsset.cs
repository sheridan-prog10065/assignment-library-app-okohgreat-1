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

    public Book Book
    {
        get => _book;
        set => _book = value;
    }

    public DateTime BorrowedOn
    {
        get => _loanPeriod.BorrowedOn;
        set
        {
            _loanPeriod.BorrowedOn = value;
        }
    }

    public DateTime ReturnedOn
    {
        get => _loanPeriod.ReturnedOn;
        set
        {
            _loanPeriod.ReturnedOn = value;
        }
    }

    public DateTime DueDate
    {
        get => _loanPeriod.DueDate;
        set
        {
            _loanPeriod.DueDate = value;
        }
    }

    public LibraryAsset(int libID, Book book)
    {
        _libID = libID;
        _book = book;
        _status = AssetStatus.Available;
        _loanPeriod = new LoanPeriod();
    }

    public bool IsAvailable()
    {
        return Status == AssetStatus.Available;
    }

    public TimeSpan GetLoanDuration()
    {
        return _loanPeriod.GetLoanDuration();
    }

    public TimeSpan GetLatePeriod()
    {
        return _loanPeriod.GetLatePeriod();
    }
}