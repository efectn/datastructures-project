namespace datastructures_project.Document;

public struct Document
{
    private string _title;
    private string _url;
    private string _description;

    public Document(string title, string url, string description)
    {
        Title = title;
        Url = url;
        Description = description;
    }
    
    private static bool isURLValid(string url)
    {
        try
        {
            return Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute);
        }
        catch
        {
            return false;
        }
    }


    public string Title
    {
        get => _title;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Title cannot be empty");
            }

            if (value.Length > 50)
            {
                throw new ArgumentException("Title cannot exceed 50 characters");
            }
            
            _title = value;
        }
    }

    public string Url
    {
        get => _url;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("URL cannot be empty");
            }

            if (!isURLValid(value))
            {
                throw new ArgumentException("URL is not a valid URL");
            }
            
            _url = value;
        }
    }

    public string Description
    {
        get => _description;
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                _description = "";
            }
            
            _description = (value.Length > 200) ? value.Substring(0, 200) : value;
        }
    }
}