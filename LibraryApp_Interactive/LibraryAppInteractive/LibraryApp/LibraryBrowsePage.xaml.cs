using LibraryAppInteractive.Business_Logic;

namespace LibraryAppInteractive;

public partial class LibraryBrowsePage : ContentPage
{
    private Library _library;
    private Book _selectedBook;

    public LibraryBrowsePage()
    {   
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _library = App.ServiceProvider.GetService<Library>();
    }

    private void OnSearchBookClicked(object sender, EventArgs e)
    {
        OnSearchBook();
    }

    public void OnSearchBook()
    {
        try
        {
            string searchInput = SearchEntry.Text;

            if (string.IsNullOrWhiteSpace(searchInput))
            {
                DisplayAlert("Error", "Please enter a book name or ISBN", "OK");
                return;
            }

            // Try to find by name first
            Book foundBook = _library.FindBookByName(searchInput);
            
            // If not found by name, try by ISBN
            if (foundBook == null)
                foundBook = _library.FindBookByISBN(searchInput);

            if (foundBook != null)
            {
                _selectedBook = foundBook;
                DisplayBookDetails(foundBook);
                DisplayBookAssets(foundBook);
                StatusLabel.Text = $"Found: {foundBook.Name}";
                StatusLabel.TextColor = Colors.Green;
                StatusLabel.IsVisible = true;
            }
            else
            {
                StatusLabel.Text = "Book not found";
                StatusLabel.TextColor = Colors.Red;
                StatusLabel.IsVisible = true;
                _selectedBook = null;
                ClearBookDetails();
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnBorrowBookClicked(object sender, EventArgs e)
    {
        OnBorrowBook();
    }

    public void OnBorrowBook()
    {
        try
        {
            if (_selectedBook == null)
            {
                DisplayAlert("Error", "Please select a book first by searching", "OK");
                return;
            }

            LibraryAsset asset = _library.BorrowBook(_selectedBook.ISBN);
            
            string message = $"Successfully borrowed '{_selectedBook.Name}'.\n" +
                           $"Due: {asset.DueDate:yyyy-MM-dd}\n" +
                           $"Use ID {asset.LibId} when returning the book.";
            
            DisplayAlert("Success", message, "OK");
            DisplayBookAssets(_selectedBook);
            StatusLabel.Text = $"Borrowed: {_selectedBook.Name}";
            StatusLabel.TextColor = Colors.Green;
            StatusLabel.IsVisible = true;
        }
        catch (InvalidOperationException ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void OnReturnBookClicked(object sender, EventArgs e)
    {
        OnReturnBook();
    }
    
    public void OnReturnBook()
    {
        try
        {
            if (_selectedBook == null)
            {
                DisplayAlert("Error", "Please select a book first by searching", "OK");
                return;
            }

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                string assetIDStr = await DisplayPromptAsync(
                    "Return Book",
                    "Enter the Asset ID to return:");

                if (!string.IsNullOrWhiteSpace(assetIDStr) && int.TryParse(assetIDStr, out int libID))
                {
                    try
                    {
                        (TimeSpan duration, int daysLate, decimal lateFees) = 
                            _library.ReturnBook(_selectedBook.ISBN, libID);

                        string message = $"Book returned successfully!\n" +
                                       $"Loaned for {duration.Days} days.";
                        
                        if (daysLate > 0)
                            message += $"\nLate by {daysLate} days.\nFee: £{lateFees:F2}";

                        DisplayAlert("Success", message, "OK");
                        DisplayBookAssets(_selectedBook);
                        StatusLabel.Text = "Book returned successfully";
                        StatusLabel.TextColor = Colors.Green;
                        StatusLabel.IsVisible = true;
                    }
                    catch (InvalidOperationException ex)
                    {
                        DisplayAlert("Error", ex.Message, "OK");
                    }
                }
            });
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void DisplayBookDetails(Book book)
    {
        BookNameLabel.Text = book.Name ?? "N/A";
        BookISBNLabel.Text = book.ISBN ?? "N/A";
        BookAuthorsLabel.Text = book.Authors.Count > 0 ? string.Join(", ", book.Authors) : "N/A";
        BookTypeLabel.Text = book.GetType().Name ?? "N/A";

        (bool isAvailable, DateTime? nextDate) = book.CheckAvailability();
        if (isAvailable)
            AvailabilityLabel.Text = $"{book.Name} is available.";
        else
            AvailabilityLabel.Text = $"Not available. Next available: {nextDate:yyyy-MM-dd}";
    }

    private void ClearBookDetails()
    {
        BookNameLabel.Text = "N/A";
        BookISBNLabel.Text = "N/A";
        BookAuthorsLabel.Text = "N/A";
        BookTypeLabel.Text = "N/A";
        AvailabilityLabel.Text = "N/A";
        AssetsCollectionView.ItemsSource = null;
    }

    private void DisplayBookAssets(Book book)
    {
        if (book?.Assets != null && book.Assets.Count > 0)
        {
            AssetsCollectionView.ItemsSource = new List<LibraryAsset>(book.Assets);
        }
        else
        {
            AssetsCollectionView.ItemsSource = null;
        }
    }
}