namespace LibraryAppInteractive.Business_Logic;

public class PaperBook : Book
{
    private const int MAX_BORROW_DAYS = 30;
    private const float LATE_PENALTY_PER_DAY = 0.25f;

    public PaperBook(string bookName, string bookISBN) : base(bookName, bookISBN, BookType.Paper)
    {
    }

    /// <summary>
    /// Overrides the base implementation to use the deadline for paper books (30 days).
    /// </summary>
    public override LibraryAsset BorrowBook()
    {
        LibraryAsset libAsset = base.BorrowBook();

        // Adjust due date according to paper book policy
        libAsset.DueDate = DateTime.Now.AddDays(MAX_BORROW_DAYS);

        return libAsset;
    }

    /// <summary>
    /// Returns a borrowed asset and calculates late fees according to paper book policy.
    /// </summary>
    public override (TimeSpan, int, decimal) ReturnBook(int libID)
    {
        (TimeSpan loanDuration, int daysLate, decimal lateFees) = base.ReturnBook(libID);

        return ((TimeSpan, int, decimal))(loanDuration, daysLate, daysLate * LATE_PENALTY_PER_DAY);
    }
}