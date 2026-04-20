using LibraryAppInteractive.Business_Logic;

namespace LibraryAppInteractive;

public partial class LibraryAdminPage : ContentPage
{
    private Library _library;

    public LibraryAdminPage()
    {        
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _library = App.ServiceProvider.GetService<Library>();
        RefreshBooks();
    }

    private void OnRegisterBookClicked(object sender, EventArgs e)
    {
        OnRegisterBook();
    }

    public void OnRegisterBook()
    {
        try
        {
            string bookName = BookNameEntry.Text;
            string bookISBN = BookISBNEntry.Text;
            string authorsText = BookAuthorsEntry.Text;
            string bookTypeText = BookTypePicker.SelectedItem as string;
            string copiesText = NCopiesEntry.Text;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(bookName))
            {
                DisplayAlert("Error", "Please enter a book name", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(bookISBN))
            {
                DisplayAlert("Error", "Please enter an ISBN", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(authorsText))
            {
                DisplayAlert("Error", "Please enter at least one author", "OK");
                return;
            }

            if (bookTypeText == null)
            {
                DisplayAlert("Error", "Please select a book type", "OK");
                return;
            }

            if (!int.TryParse(copiesText, out int nCopies) || nCopies <= 0)
            {
                DisplayAlert("Error", "Please enter a valid number of copies", "OK");
                return;
            }

            // Parse book type
            BookType bookType = bookTypeText switch
            {
                "Paper" => BookType.Paper,
                "Digital" => BookType.Digital,
                "Audio" => BookType.Audio,
                _ => BookType.Paper
            };

            // Parse authors
            string[] authors = authorsText.Split(',')
                .Select(a => a.Trim())
                .Where(a => !string.IsNullOrWhiteSpace(a))
                .ToArray();

            // Register book
            _library.RegisterBook(bookName, bookISBN, authors, bookType, nCopies);

            DisplayAlert("Success", "Book added successfully!", "OK");

            // Clear inputs
            BookNameEntry.Text = string.Empty;
            BookISBNEntry.Text = string.Empty;
            BookAuthorsEntry.Text = string.Empty;
            BookTypePicker.SelectedIndex = -1;
            NCopiesEntry.Text = string.Empty;

            // Refresh books list
            RefreshBooks();
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

    private void OnDisplayBookAssetsClicked(object sender, EventArgs e)
    {
        OnDisplayBookAssets();
    }

    public void OnDisplayBookAssets()
    {
        try
        {
            if (BooksCollectionView.SelectedItem is not Book selectedBook)
            {
                DisplayAlert("Error", "Please select a book from the list", "OK");
                return;
            }

            string assetInfo = $"Library Assets for: {selectedBook.Name}\n\n";

            if (selectedBook.Assets.Count == 0)
            {
                assetInfo += "No assets found for this book.";
            }
            else
            {
                foreach (var asset in selectedBook.Assets)
                {
                    assetInfo += $"Asset ID {asset.LibId}: {asset.Status}\n";
                    
                    if (asset.Status == AssetStatus.Loaned)
                    {
                        assetInfo += $"  Borrowed: {asset.BorrowedOn:yyyy-MM-dd}\n";
                        assetInfo += $"  Due: {asset.DueDate:yyyy-MM-dd}\n";
                    }
                }
            }

            DisplayAlert("Assets", assetInfo, "OK");
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private void RefreshBooks()
    {
        try
        {
            var books = _library.GetAllBooks();
            BooksCollectionView.ItemsSource = null;
            BooksCollectionView.ItemsSource = books;
        }
        catch (Exception ex)
        {
            DisplayAlert("Error", $"Failed to load books: {ex.Message}", "OK");
        }
    }
}