namespace LibraryAppInteractive.Business_Logic;

public struct LoanPeriod
{
    private DateTime _borrowedOn;
    private DateTime _returnedOn;
    private DateTime _dueDate;
    
    public DateTime BorrowedOn
    {
        get => _borrowedOn;
        set => _borrowedOn = value;
    }
    public DateTime ReturnedOn
    {
        get => _returnedOn;
        set => _returnedOn = value;
    }

    public DateTime DueDate
    {
        get => _dueDate;
        set => _dueDate = value;
    }
    
    public LoanPeriod(DateTime borrowedOn, DateTime dueDate)
    {
        _borrowedOn = borrowedOn;
        _dueDate = dueDate;
        _returnedOn = DateTime.MinValue;
    }
}