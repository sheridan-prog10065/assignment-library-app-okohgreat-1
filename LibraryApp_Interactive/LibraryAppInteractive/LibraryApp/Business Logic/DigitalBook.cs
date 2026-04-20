namespace LibraryAppInteractive.Business_Logic;

public class DigitalBook : Book
{
    private int _maxBorrowDays;
    private float _latePenaltyPerDay;
    private Random _random = new Random();

    public DigitalBook(string bookName, string bookISBN) : base(bookName, bookISBN, BookType.Digital)
    {
        _maxBorrowDays = 0;
        _latePenaltyPerDay = 0.0f;

        DetermineLoanLicense();
    }

    /// <summary>
    /// Determine the values for maximum loan duration and late penalty according to license agreement using randomly generated values.
    /// Maximum loan duration: 2-8 weeks (randomly)
    /// Late penalty: 0.1 - 0.5 per day (randomly)
    /// </summary>
    private void DetermineLoanLicense()
    {
        _maxBorrowDays = _random.Next(2 * 7, 8 * 7 + 1); // 2-8 weeks in days
        _latePenaltyPerDay = 0.1f + (float)_random.NextDouble() * 0.4f; // 0.1 to 0.5
    }

    /// <summary>
    /// Overrides the base implementation to use the deadline for digital books.
    /// </summary>
    public override LibraryAsset BorrowBook()
    {
        LibraryAsset libAsset = base.BorrowBook();

        // Adjust due date according to digital book policy
        libAsset.DueDate = DateTime.Now.AddDays(_maxBorrowDays);

        return libAsset;
    }

    /// <summary>
    /// Returns a borrowed asset and calculates late fees according to digital book policy.
    /// </summary>
    public override (TimeSpan, int, decimal) ReturnBook(int libID)
    {
        (TimeSpan loanDuration, int daysLate, decimal lateFees) = base.ReturnBook(libID);

        return (loanDuration, daysLate, (decimal)(daysLate * _latePenaltyPerDay));
    }
}